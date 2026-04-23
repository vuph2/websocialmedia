namespace web.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string PostType { get; set; } = "text";
        public string? Feeling { get; set; }
        public string Privacy { get; set; } = "public";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // FK
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        // Navigation
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PollOption> PollOptions { get; set; } = new List<PollOption>();
        public ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
    }

    public class PostMedia
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
    }

    public class PostLike
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
        public string ReactionType { get; set; } = "like"; // like, love, haha, wow, sad, angry
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }

    public class PollOption
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public ICollection<PollVote> Votes { get; set; } = new List<PollVote>();
    }

    public class PollVote
    {
        public int Id { get; set; }
        public int PollOptionId { get; set; }
        public PollOption PollOption { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }

    // ── Chat / Direct Messaging ───────────────────────────────────
    public class Conversation
    {
        public int Id { get; set; }

        // User1Id is always the lexically smaller ID for uniqueness
        public string User1Id { get; set; } = string.Empty;
        public ApplicationUser User1 { get; set; } = null!;

        public string User2Id { get; set; } = string.Empty;
        public ApplicationUser User2 { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    public class ChatMessage
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public string SenderId { get; set; } = string.Empty;
        public ApplicationUser Sender { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; } // image, video, file
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
