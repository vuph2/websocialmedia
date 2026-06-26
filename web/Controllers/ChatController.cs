using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Hubs;
using web.Models;

namespace web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET /Chat or /Chat/Index
        public async Task<IActionResult> Index(int? conversationId = null, string? userId = null)
        {
            var currentUserId = _userManager.GetUserId(User)!;

            var blockedUserIds = await _db.BlockedUsers
                .Where(b => b.BlockerId == currentUserId || b.BlockedUserId == currentUserId)
                .Select(b => b.BlockerId == currentUserId ? b.BlockedUserId : b.BlockerId)
                .ToListAsync();

            var vm = new ChatPageViewModel { CurrentUserId = currentUserId };

            // Load all conversations for current user, excluding blocked users
            var conversations = await _db.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(c => (c.User1Id == currentUserId || c.User2Id == currentUserId)
                            && !blockedUserIds.Contains(c.User1Id == currentUserId ? c.User2Id : c.User1Id))
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            vm.Conversations = conversations.Select(c =>
            {
                var other = c.User1Id == currentUserId ? c.User2 : c.User1;
                var lastMsg = c.Messages.FirstOrDefault();
                var unread = _db.ChatMessages.Count(m =>
                    m.ConversationId == c.Id && m.SenderId != currentUserId && !m.IsRead);

                return new ConversationListViewModel
                {
                    ConversationId = c.Id,
                    OtherUserId = other.Id,
                    OtherUserName = $"{other.FirstName} {other.LastName}".Trim(),
                    OtherUserAvatar = other.ProfilePictureUrl,
                    IsOnline = NexusHub.IsUserOnline(other.Id),
                    LastMessage = lastMsg?.Content,
                    LastMessageAt = lastMsg?.SentAt ?? c.CreatedAt,
                    UnreadCount = unread
                };
            }).ToList();

            // If userId is provided, start/open conversation with that user
            if (!string.IsNullOrEmpty(userId) && userId != currentUserId)
            {
                if (blockedUserIds.Contains(userId))
                {
                    return RedirectToAction("Index");
                }

                var user1 = string.Compare(currentUserId, userId, StringComparison.Ordinal) < 0 ? currentUserId : userId;
                var user2 = user1 == currentUserId ? userId : currentUserId;

                var conv = await _db.Conversations
                    .FirstOrDefaultAsync(c => c.User1Id == user1 && c.User2Id == user2);

                if (conv == null)
                {
                    conv = new Conversation
                    {
                        User1Id = user1,
                        User2Id = user2,
                        CreatedAt = DateTime.UtcNow,
                        LastMessageAt = DateTime.UtcNow
                    };
                    _db.Conversations.Add(conv);
                    await _db.SaveChangesAsync();

                    // Add to conversation list if new
                    var otherUser = await _userManager.FindByIdAsync(userId);
                    if (otherUser != null)
                    {
                        vm.Conversations.Insert(0, new ConversationListViewModel
                        {
                            ConversationId = conv.Id,
                            OtherUserId = otherUser.Id,
                            OtherUserName = $"{otherUser.FirstName} {otherUser.LastName}".Trim(),
                            OtherUserAvatar = otherUser.ProfilePictureUrl,
                            IsOnline = NexusHub.IsUserOnline(otherUser.Id),
                            LastMessageAt = conv.CreatedAt,
                            UnreadCount = 0
                        });
                    }
                }

                conversationId = conv.Id;
            }

            // Load active conversation messages
            if (conversationId.HasValue)
            {
                var conv = await _db.Conversations
                    .Include(c => c.User1)
                    .Include(c => c.User2)
                    .FirstOrDefaultAsync(c => c.Id == conversationId.Value
                        && (c.User1Id == currentUserId || c.User2Id == currentUserId));

                if (conv != null)
                {
                    var otherId = conv.User1Id == currentUserId ? conv.User2Id : conv.User1Id;
                    if (blockedUserIds.Contains(otherId))
                    {
                        return RedirectToAction("Index");
                    }
                    var other = conv.User1Id == currentUserId ? conv.User2 : conv.User1;
                    vm.ActiveConversationId = conv.Id;
                    vm.ActiveRecipientId = other.Id;
                    vm.ActiveRecipientName = $"{other.FirstName} {other.LastName}".Trim();
                    vm.ActiveRecipientAvatar = other.ProfilePictureUrl;
                    vm.ActiveRecipientOnline = NexusHub.IsUserOnline(other.Id);

                    var messages = await _db.ChatMessages
                        .Include(m => m.Sender)
                        .Where(m => m.ConversationId == conv.Id)
                        .OrderBy(m => m.SentAt)
                        .Take(50)
                        .ToListAsync();

                    vm.Messages = messages.Select(m => new ChatMessageViewModel
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}".Trim(),
                        SenderAvatar = m.Sender.ProfilePictureUrl,
                        Content = string.IsNullOrEmpty(m.Content) ? string.Empty : Helpers.HtmlFormatter.FormatPostContent(m.Content).ToString(),
                        MediaUrl = m.MediaUrl,
                        MediaType = m.MediaType,
                        SentAt = m.SentAt,
                        IsRead = m.IsRead,
                        IsMine = m.SenderId == currentUserId
                    }).ToList();

                    // Mark unread messages as read
                    var unread = await _db.ChatMessages
                        .Where(m => m.ConversationId == conv.Id && m.SenderId != currentUserId && !m.IsRead)
                        .ToListAsync();
                    foreach (var m in unread) m.IsRead = true;
                    if (unread.Any()) await _db.SaveChangesAsync();
                }
            }

            return View(vm);
        }

        // GET /Chat/GetMessages?conversationId=X&before=timestamp
        [HttpGet]
        public async Task<IActionResult> GetMessages(int conversationId, DateTime? before = null)
        {
            var currentUserId = _userManager.GetUserId(User)!;

            // Verify user is part of conversation
            var conv = await _db.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId
                    && (c.User1Id == currentUserId || c.User2Id == currentUserId));

            if (conv == null) return Forbid();

            var query = _db.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId);

            if (before.HasValue)
                query = query.Where(m => m.SentAt < before.Value);

            var messages = await query
                .OrderByDescending(m => m.SentAt)
                .Take(20)
                .ToListAsync();

            var result = messages.OrderBy(m => m.SentAt).Select(m => new
            {
                id = m.Id,
                senderId = m.SenderId,
                senderName = $"{m.Sender.FirstName} {m.Sender.LastName}".Trim(),
                senderAvatar = m.Sender.ProfilePictureUrl,
                content = string.IsNullOrEmpty(m.Content) ? string.Empty : Helpers.HtmlFormatter.FormatPostContent(m.Content).ToString(),
                mediaUrl = m.MediaUrl,
                mediaType = m.MediaType,
                sentAt = m.SentAt.ToString("o"),
                isRead = m.IsRead,
                isMine = m.SenderId == currentUserId
            }).ToList();

            return Json(new { messages = result, hasMore = messages.Count == 20 });
        }

        // GET /Chat/GetUnreadCount
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var currentUserId = _userManager.GetUserId(User)!;

            var count = await _db.ChatMessages
                .Where(m => m.SenderId != currentUserId && !m.IsRead
                    && (m.Conversation.User1Id == currentUserId || m.Conversation.User2Id == currentUserId))
                .CountAsync();

            return Json(new { count });
        }

        // GET /Chat/SearchUsers?q=query
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(new List<object>());

            var currentUserId = _userManager.GetUserId(User)!;
            var queryLower = q.ToLower().Trim();

            var blockedUserIds = await _db.BlockedUsers
                .Where(b => b.BlockerId == currentUserId || b.BlockedUserId == currentUserId)
                .Select(b => b.BlockerId == currentUserId ? b.BlockedUserId : b.BlockerId)
                .ToListAsync();

            var users = await _userManager.Users
                .Where(u => u.Id != currentUserId && !blockedUserIds.Contains(u.Id) &&
                    ((u.FirstName + " " + u.LastName).ToLower().Contains(queryLower)
                    || u.UserName!.ToLower().Contains(queryLower)
                    || u.Email!.ToLower().Contains(queryLower)))
                .Take(10)
                .Select(u => new
                {
                    id = u.Id,
                    name = (u.FirstName + " " + u.LastName).Trim(),
                    avatar = u.ProfilePictureUrl,
                    isOnline = NexusHub.IsUserOnline(u.Id)
                })
                .ToListAsync();

            return Json(users);
        }

        // POST /Chat/UploadChatMedia
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadChatMedia(IFormFile file)
        {
            if (file == null || file.Length == 0) return Json(new { success = false });

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "chat");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var type = "file";
            if (file.ContentType.StartsWith("image/")) type = "image";
            else if (file.ContentType.StartsWith("video/")) type = "video";
            else if (file.ContentType.StartsWith("audio/")) type = "audio";

            return Json(new { success = true, url = $"/uploads/chat/{fileName}", type });
        }
    }
}
