using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.Models
{
    public class Report
    {
        public int Id { get; set; }

        // Who reported
        [Required]
        public string ReporterId { get; set; } = string.Empty;
        [ForeignKey("ReporterId")]
        public ApplicationUser Reporter { get; set; } = null!;

        // What was reported (post or user)
        public int? TargetPostId { get; set; }
        [ForeignKey("TargetPostId")]
        public Post? TargetPost { get; set; }

        public string? TargetUserId { get; set; }
        [ForeignKey("TargetUserId")]
        public ApplicationUser? TargetUser { get; set; }

        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        // Status: Pending / Resolved / Dismissed
        public string Status { get; set; } = "Pending";

        public string? ModeratorNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedAt { get; set; }

        public string? ResolvedById { get; set; }
        [ForeignKey("ResolvedById")]
        public ApplicationUser? ResolvedBy { get; set; }
    }
}
