using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using web.Data;
using web.Models;

namespace web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.UtcNow.Date;
            var vm = new AdminDashboardViewModel
            {
                TotalUsers    = await _db.Users.CountAsync(),
                TotalPosts    = await _db.Posts.CountAsync(),
                TotalComments = await _db.Comments.CountAsync(),
                TotalMessages = await _db.ChatMessages.CountAsync(),
                PendingReports = await _db.Reports.CountAsync(r => r.Status == "Pending"),
                BlockedUsers  = await _db.Users.CountAsync(u => u.IsBlocked),
                NewUsersToday = await _db.Users.CountAsync(u => u.CreatedAt >= today),
                NewPostsToday = await _db.Posts.CountAsync(p => p.CreatedAt >= today),
            };

            // Recent 5 users
            var recentUsers = await _db.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            foreach (var u in recentUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var postCount = await _db.Posts.CountAsync(p => p.UserId == u.Id);
                var reportCount = await _db.Reports.CountAsync(r => r.TargetUserId == u.Id);
                vm.RecentUsers.Add(new AdminUserViewModel
                {
                    Id          = u.Id,
                    FullName    = $"{u.FirstName} {u.LastName}".Trim(),
                    Email       = u.Email ?? "",
                    Username    = u.UserName,
                    Avatar      = u.ProfilePictureUrl,
                    CreatedAt   = u.CreatedAt,
                    IsBlocked   = u.IsBlocked,
                    BlockedReason = u.BlockedReason,
                    BlockedAt   = u.BlockedAt,
                    Roles       = roles.ToList(),
                    PostCount   = postCount,
                    ReportCount = reportCount
                });
            }

            // Recent 5 pending reports
            var recentReports = await _db.Reports
                .Include(r => r.Reporter)
                .Include(r => r.TargetPost).ThenInclude(p => p != null ? p.User : null)
                .Include(r => r.TargetUser)
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            vm.RecentReports = recentReports.Select(r => MapToQueueVm(r)).ToList();

            return View(vm);
        }

        // GET /Admin/Users
        public async Task<IActionResult> Users(string? search, string? filter, int page = 1)
        {
            const int pageSize = 20;
            var query = _db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(u =>
                    (u.FirstName + " " + u.LastName).ToLower().Contains(search) ||
                    (u.Email != null && u.Email.ToLower().Contains(search)) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(search)));
            }

            if (filter == "blocked")
                query = query.Where(u => u.IsBlocked);

            var total = await query.CountAsync();
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vmList = new List<AdminUserViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var postCount = await _db.Posts.CountAsync(p => p.UserId == u.Id);
                var reportCount = await _db.Reports.CountAsync(r => r.TargetUserId == u.Id);
                vmList.Add(new AdminUserViewModel
                {
                    Id          = u.Id,
                    FullName    = $"{u.FirstName} {u.LastName}".Trim(),
                    Email       = u.Email ?? "",
                    Username    = u.UserName,
                    Avatar      = u.ProfilePictureUrl,
                    CreatedAt   = u.CreatedAt,
                    IsBlocked   = u.IsBlocked,
                    BlockedReason = u.BlockedReason,
                    BlockedAt   = u.BlockedAt,
                    Roles       = roles.ToList(),
                    PostCount   = postCount,
                    ReportCount = reportCount
                });
            }

            ViewBag.Search      = search;
            ViewBag.Filter      = filter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.TotalUsers  = total;
            return View(vmList);
        }

        // POST /Admin/BlockUser
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(string userId, string reason)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsBlocked   = true;
            user.BlockedAt   = DateTime.UtcNow;
            user.BlockedReason = reason;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"Tài khoản \"{user.UserName}\" đã bị khóa.";
            return RedirectToAction("Users");
        }

        // POST /Admin/UnblockUser
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsBlocked   = false;
            user.BlockedAt   = null;
            user.BlockedReason = null;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"Tài khoản \"{user.UserName}\" đã được mở khóa.";
            return RedirectToAction("Users");
        }

        // POST /Admin/ChangeRole
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Don't demote yourself
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == currentUserId)
            {
                TempData["Error"] = "Bạn không thể tự thay đổi vai trò của mình.";
                return RedirectToAction("Users");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["Success"] = $"Vai trò của \"{user.UserName}\" đã được chuyển thành \"{newRole}\".";
            return RedirectToAction("Users");
        }

        // GET /Admin/Reports
        public async Task<IActionResult> Reports(string? status, int page = 1)
        {
            const int pageSize = 20;
            var query = _db.Reports
                .Include(r => r.Reporter)
                .Include(r => r.TargetPost).ThenInclude(p => p != null ? p.User : null)
                .Include(r => r.TargetUser)
                .Include(r => r.ResolvedBy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            var total = await query.CountAsync();
            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Status      = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.TotalReports = total;
            return View(reports.Select(r => MapToQueueVm(r)).ToList());
        }

        // POST /Admin/ResolveReport
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveReport(int reportId, string action, string? note)
        {
            var report = await _db.Reports
                .Include(r => r.TargetPost)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            report.Status       = action == "remove" ? "Resolved" : "Dismissed";
            report.ResolvedAt   = DateTime.UtcNow;
            report.ResolvedById = currentUserId;
            report.ModeratorNote = note;

            if (action == "remove" && report.TargetPost != null)
            {
                var post = report.TargetPost;

                // Notify post author
                var notif = new Notification
                {
                    UserId     = post.UserId,
                    Content    = "Một bài viết của bạn đã bị xóa do vi phạm chính sách cộng đồng.",
                    Type       = "moderation",
                    RelatedUrl = "/",
                    CreatedAt  = DateTime.UtcNow
                };
                _db.Notifications.Add(notif);

                _db.Posts.Remove(post);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = action == "remove" ? "Bài viết đã bị xóa." : "Báo cáo đã bị bác bỏ.";
            return RedirectToAction("Reports");
        }

        // Helper
        private static ModerationQueueViewModel MapToQueueVm(Report r) => new()
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
        };
    }
}
