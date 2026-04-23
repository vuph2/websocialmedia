namespace web.Models
{
    public class StoryLike
    {
        public int Id { get; set; }
        public int StoryId { get; set; }
        public Story Story { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
        public string ReactionType { get; set; } = "like"; // like, love, haha, wow, sad, angry
    }
}
