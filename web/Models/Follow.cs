using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }

        public string FollowerId { get; set; } = string.Empty;
        public string FollowedId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("FollowerId")]
        public ApplicationUser Follower { get; set; } = null!;

        [ForeignKey("FollowedId")]
        public ApplicationUser Followed { get; set; } = null!;
    }
}
