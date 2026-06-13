using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using web.Data;
using web.Models;

namespace web.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class ModerationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ModerationController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET /Moderation/Queue
        public async Task<IActionResult> Queue(string? status)
        {
            status ??= "Pending";
            var reports = await _db.Reports
                .Include(r => r.Reporter)
                .Include(r => r.TargetPost).ThenInclude(p => p != null ? p.User : null)
                .Include(r => r.TargetUser)
                .Include(r => r.ResolvedBy)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var vm = reports.Select(r => new ModerationQueueViewModel
            {
                ReportId       = r.Id,
                ReporterId     = r.ReporterId,
                ReporterName   = r.Reporter != null ? $"{r.Reporter.FirstName} {r.Reporter.LastName}".Trim() : "Unknown",
                ReporterAvatar = r.Reporter?.ProfilePictureUrl,
                PostId         = r.TargetPostId,
                PostContent    = r.TargetPost?.Content,
                PostAuthorName = r.TargetPost?.User != null ? $"{r.TargetPost.User.FirstName} {r.TargetPost.User.LastName}".Trim() : null,
                PostAuthorId   = r.TargetPost?.UserId,
                TargetUserId   = r.TargetUserId,
                TargetUserName = r.TargetUser != null ? $"{r.TargetUser.FirstName} {r.TargetUser.LastName}".Trim() : null,
                Reason         = r.Reason,
                Status         = r.Status,
                ModeratorNote  = r.ModeratorNote,
                CreatedAt      = r.CreatedAt,
                ResolvedAt     = r.ResolvedAt,
                ResolvedByName = r.ResolvedBy != null ? $"{r.ResolvedBy.FirstName} {r.ResolvedBy.LastName}".Trim() : null
            }).ToList();

            ViewBag.Status          = status;
            ViewBag.PendingCount    = await _db.Reports.CountAsync(r => r.Status == "Pending");
            ViewBag.ResolvedCount   = await _db.Reports.CountAsync(r => r.Status == "Resolved");
            ViewBag.DismissedCount  = await _db.Reports.CountAsync(r => r.Status == "Dismissed");
            return View(vm);
        }

        // POST /Moderation/Process
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int reportId, string action, string? note)
        {
            var report = await _db.Reports
                .Include(r => r.TargetPost)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            report.ResolvedById  = currentUserId;
            report.ResolvedAt    = DateTime.UtcNow;
            report.ModeratorNote = note;

            if (action == "remove" && report.TargetPost != null)
            {
                report.Status = "Resolved";
                var post = report.TargetPost;

                // Notify post author
                _db.Notifications.Add(new Notification
                {
                    UserId     = post.UserId,
                    Content    = "Bài viết của bạn đã bị xóa do vi phạm chính sách cộng đồng.",
                    Type       = "moderation",
                    RelatedUrl = "/",
                    CreatedAt  = DateTime.UtcNow
                });

                _db.Posts.Remove(post);
            }
            else
            {
                report.Status = "Dismissed";
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = action == "remove" ? "Bài viết đã bị xóa thành công." : "Báo cáo đã được đánh dấu không hợp lệ.";
            return RedirectToAction("Queue");
        }
    }
}
