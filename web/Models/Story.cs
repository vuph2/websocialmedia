namespace web.Models
{
    public class Story
    {
        public int Id { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = "image"; // "image" or "video"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);

        public ICollection<StoryLike> StoryLikes { get; set; } = new List<StoryLike>();
        public ICollection<StoryView> StoryViews { get; set; } = new List<StoryView>();
    }
}
