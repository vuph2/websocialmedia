using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Claims;
using web.Data;
using web.Models;
using web.Helpers;

namespace web.Hubs
{
    [Authorize]
    public class NexusHub : Hub
    {
        // Maps userId → set of connectionIds (a user can have multiple tabs)
        private static readonly ConcurrentDictionary<string, HashSet<string>> _onlineUsers = new();

        private readonly ApplicationDbContext _db;

        public NexusHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                _onlineUsers.AddOrUpdate(
                    userId,
                    _ => new HashSet<string> { Context.ConnectionId },
                    (_, set) => { lock (set) { set.Add(Context.ConnectionId); } return set; }
                );

                // Join a group named after the userId for easy targeted messaging
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                if (_onlineUsers.TryGetValue(userId, out var connections))
                {
                    lock (connections)
                    {
                        connections.Remove(Context.ConnectionId);
                        if (connections.Count == 0)
                            _onlineUsers.TryRemove(userId, out _);
                    }
                }

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Send a direct message to another user. Persists to DB and broadcasts in real-time.
        /// </summary>
        public async Task SendDirectMessage(string recipientId, string content, string? mediaUrl = null, string? mediaType = null)
        {
            var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId) || (string.IsNullOrEmpty(content?.Trim()) && string.IsNullOrEmpty(mediaUrl)))
                return;

            // Prevent messages if either party has blocked the other
            var isBlocked = await _db.BlockedUsers.AnyAsync(b =>
                (b.BlockerId == senderId && b.BlockedUserId == recipientId) ||
                (b.BlockerId == recipientId && b.BlockedUserId == senderId));
            if (isBlocked)
                return;

            content = content?.Trim() ?? string.Empty;
            if (content.Length > 2000) content = content[..2000];

            // Normalize user ordering for unique conversation lookup
            var user1Id = string.Compare(senderId, recipientId, StringComparison.Ordinal) < 0 ? senderId : recipientId;
            var user2Id = user1Id == senderId ? recipientId : senderId;

            // Find or create conversation
            var conversation = await _db.Conversations
                .FirstOrDefaultAsync(c => c.User1Id == user1Id && c.User2Id == user2Id);

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1Id = user1Id,
                    User2Id = user2Id,
                    CreatedAt = DateTime.UtcNow,
                    LastMessageAt = DateTime.UtcNow
                };
                _db.Conversations.Add(conversation);
                await _db.SaveChangesAsync();
            }
            else
            {
                conversation.LastMessageAt = DateTime.UtcNow;
            }

            // Save message
            var message = new ChatMessage
            {
                ConversationId = conversation.Id,
                SenderId = senderId,
                Content = content,
                MediaUrl = mediaUrl,
                MediaType = mediaType,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };
            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();

            // Get sender info for display
            var sender = await _db.Users.FindAsync(senderId);
            var senderName = sender != null ? $"{sender.FirstName} {sender.LastName}".Trim() : "User";
            var senderAvatar = sender?.ProfilePictureUrl;

            // Format content if it exists
            var formattedContent = string.IsNullOrEmpty(content) ? string.Empty : HtmlFormatter.FormatPostContent(content).ToString();

            var payload = new
            {
                id = message.Id,
                conversationId = conversation.Id,
                senderId,
                senderName,
                senderAvatar,
                content = formattedContent,
                mediaUrl = message.MediaUrl,
                mediaType = message.MediaType,
                sentAt = message.SentAt.ToString("o"),
                isRead = false
            };

            // Send to recipient
            await Clients.Group(recipientId).SendAsync("ReceiveDirectMessage", payload);

            // Send confirmation back to sender (all their tabs)
            await Clients.Group(senderId).SendAsync("ReceiveDirectMessage", payload);
        }

        /// <summary>
        /// Mark messages as read – called when a user opens a conversation.
        /// </summary>
        public async Task MarkConversationRead(int conversationId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return;

            var unread = await _db.ChatMessages
                .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
                .ToListAsync();

            foreach (var m in unread) m.IsRead = true;
            await _db.SaveChangesAsync();

            // Notify the sender that their messages were read
            if (unread.Any())
            {
                var otherUserId = unread.First().SenderId;
                await Clients.Group(otherUserId).SendAsync("MessagesRead", new
                {
                    conversationId,
                    readByUserId = userId
                });
            }
        }

        /// <summary>
        /// Notify the other user that this user is typing.
        /// </summary>
        public async Task SendTypingIndicator(string recipientId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return;

            var sender = await _db.Users.FindAsync(userId);
            var name = sender != null ? $"{sender.FirstName} {sender.LastName}".Trim() : "Someone";

            await Clients.Group(recipientId).SendAsync("UserTyping", new
            {
                userId,
                name
            });
        }

        /// <summary>
        /// Check if a user is currently online.
        /// </summary>
        public static bool IsUserOnline(string userId)
            => _onlineUsers.ContainsKey(userId);
    }
}
