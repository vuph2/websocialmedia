namespace web.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        
        public string RequesterId { get; set; } = string.Empty;
        public ApplicationUser Requester { get; set; } = null!;

        public string ReceiverId { get; set; } = string.Empty;
        public ApplicationUser Receiver { get; set; } = null!;

        public string Status { get; set; } = "Pending"; // "Pending", "Accepted"
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
    }
}
