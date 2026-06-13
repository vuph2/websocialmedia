using Microsoft.AspNetCore.Identity;

namespace web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Account suspension
        public bool IsBlocked { get; set; } = false;
        public DateTime? BlockedAt { get; set; }
        public string? BlockedReason { get; set; }

        // Navigation
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        // New properties for Friendships & Stories
        public ICollection<Friendship> FriendshipsRequested { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendshipsReceived { get; set; } = new List<Friendship>();
        public ICollection<Story> Stories { get; set; } = new List<Story>();
        public ICollection<StoryLike> StoryLikes { get; set; } = new List<StoryLike>();
        public ICollection<StoryView> StoryViews { get; set; } = new List<StoryView>();

        // Block relationships
        public ICollection<BlockedUser> BlockedByMe { get; set; } = new List<BlockedUser>();
        public ICollection<BlockedUser> BlockedMe { get; set; } = new List<BlockedUser>();
    }
}
