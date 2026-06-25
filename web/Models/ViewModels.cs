using System.ComponentModel.DataAnnotations;

namespace web.Models
{
    // ── Auth ─────────────────────────────────────────────────────
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginPageViewModel
    {
        public LoginViewModel Login { get; set; } = new();
        public RegisterViewModel Register { get; set; } = new();
        public string ActiveTab { get; set; } = "login";
        public string? ErrorMessage { get; set; }
    }

    // ── Post ViewModels ───────────────────────────────────────────
    public class CreatePostViewModel
    {
        [MaxLength(2000)]
        public string? Content { get; set; }
        public string PostType { get; set; } = "text";
        public string Privacy { get; set; } = "public";
        public string? Feeling { get; set; }
        public List<IFormFile> MediaFiles { get; set; } = new List<IFormFile>();

        // Poll
        public List<string> PollOptions { get; set; } = new();
    }

    public class PollOptionViewModel
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public bool VotedByCurrentUser { get; set; }
        public double Percentage { get; set; }
    }

    public class PostFeedViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string PostType { get; set; } = "text";
        public string? Feeling { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public string? CurrentUserReaction { get; set; } // like, love, haha, etc.
        public bool IsOwnPost { get; set; }
        public List<CommentViewModel> RecentComments { get; set; } = new();
        public List<PollOptionViewModel> PollOptions { get; set; } = new();
        public bool HasVoted { get; set; }
        public bool IsFollowedByAuthor { get; set; } // Current user follows author
    }

    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOwnComment { get; set; }
    }

    public class StoryFeedViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = "image";
        public DateTime CreatedAt { get; set; }
        public bool IsOwn { get; set; }
        public int ViewCount { get; set; }
        public List<StoryStatViewModel> Reactions { get; set; } = new();
        public List<StoryStatViewModel> Viewers { get; set; } = new();
    }

    public class StoryStatViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public string? ReactionType { get; set; } // null if just a view
    }

    public class UserStoriesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public List<StoryFeedViewModel> Stories { get; set; } = new();
    }

    public class FeedPageViewModel
    {
        public List<UserStoriesViewModel> UserStories { get; set; } = new();
        public List<PostFeedViewModel> Posts { get; set; } = new();
        public CreatePostViewModel NewPost { get; set; } = new();
        public List<SuggestUserViewModel> SuggestedUsers { get; set; } = new();
    }

    public class SuggestUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public bool IsFollowed { get; set; }
    }

    // ── Profile ───────────────────────────────────────────────────
    public class ProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostCount { get; set; }
        public string FriendshipStatus { get; set; } = "None"; // "None", "PendingReceived", "PendingSent", "Friends"
        public int FriendshipId { get; set; } // Nếu có pending hoặc friend
        public List<PostFeedViewModel> Posts { get; set; } = new();

        // For editing
        public IFormFile? NewProfilePicture { get; set; }
    }

    // ── Chat / Direct Messaging ──────────────────────────────────
    public class ConversationListViewModel
    {
        public int ConversationId { get; set; }
        public string OtherUserId { get; set; } = string.Empty;
        public string OtherUserName { get; set; } = string.Empty;
        public string? OtherUserAvatar { get; set; }
        public bool IsOnline { get; set; }
        public string? LastMessage { get; set; }
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }

    public class ChatMessageViewModel
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string? SenderAvatar { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsMine { get; set; }
    }

    public class ChatPageViewModel
    {
        public List<ConversationListViewModel> Conversations { get; set; } = new();
        public int? ActiveConversationId { get; set; }
        public string? ActiveRecipientId { get; set; }
        public string? ActiveRecipientName { get; set; }
        public string? ActiveRecipientAvatar { get; set; }
        public bool ActiveRecipientOnline { get; set; }
        public List<ChatMessageViewModel> Messages { get; set; } = new();
        public string CurrentUserId { get; set; } = string.Empty;
    }

    // ── Settings & Notifications ─────────────────────────────────
    public class SettingsViewModel
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
        public bool HasPassword { get; set; } = true;
    }

    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? RelatedUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class FriendUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public int FriendshipId { get; set; }
        public DateTime? FriendsSince { get; set; }
    }

    public class FriendsPageViewModel
    {
        public List<FriendUserViewModel> Friends { get; set; } = new();
        public List<FriendUserViewModel> PendingRequests { get; set; } = new();
    }

    // ── Report ──────────────────────────────────────────────────
    public class ReportViewModel
    {
        [Required]
        public int? PostId { get; set; }
        public string? TargetUserId { get; set; }
        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    // ── Admin ──────────────────────────────────────────────────
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
        public int TotalMessages { get; set; }
        public int PendingReports { get; set; }
        public int BlockedUsers { get; set; }
        public int NewUsersToday { get; set; }
        public int NewPostsToday { get; set; }
        public List<AdminUserViewModel> RecentUsers { get; set; } = new();
        public List<ModerationQueueViewModel> RecentReports { get; set; } = new();
    }

    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBlocked { get; set; }
        public string? BlockedReason { get; set; }
        public DateTime? BlockedAt { get; set; }
        public List<string> Roles { get; set; } = new();
        public int PostCount { get; set; }
        public int ReportCount { get; set; }
    }

    public class ModerationQueueViewModel
    {
        public int ReportId { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public string? ReporterAvatar { get; set; }
        public string ReporterId { get; set; } = string.Empty;
        public int? PostId { get; set; }
        public string? PostContent { get; set; }
        public string? PostAuthorName { get; set; }
        public string? PostAuthorId { get; set; }
        public string? TargetUserId { get; set; }
        public string? TargetUserName { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ModeratorNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? ResolvedByName { get; set; }
    }
}
