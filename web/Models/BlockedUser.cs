using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.Models
{
    public class BlockedUser
    {
        public int Id { get; set; }

        [Required]
        public string BlockerId { get; set; } = string.Empty;
        [ForeignKey("BlockerId")]
        public ApplicationUser Blocker { get; set; } = null!;

        [Required]
        public string BlockedUserId { get; set; } = string.Empty;
        [ForeignKey("BlockedUserId")]
        public ApplicationUser Blocked { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
