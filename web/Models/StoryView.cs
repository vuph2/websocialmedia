namespace web.Models
{
    public class StoryView
    {
        public int Id { get; set; }
        public int StoryId { get; set; }
        public Story Story { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
