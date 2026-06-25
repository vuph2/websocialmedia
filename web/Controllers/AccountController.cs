using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using web.Models;

namespace web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly GoogleAuthSettings _googleSettings;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            GoogleAuthSettings googleSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _googleSettings = googleSettings;
        }

        // GET /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["GoogleEnabled"] = _googleSettings.Enabled;
            return View(new LoginPageViewModel());
        }

        // POST /Account/Login
        // Read form values directly — avoids Register fields failing validation
        [HttpPost, ActionName("Login"), ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPost()
        {
            var email      = Request.Form["Login.Email"].ToString().Trim();
            var password   = Request.Form["Login.Password"].ToString();
            var rememberMe = Request.Form["Login.RememberMe"].ToString().Contains("true");

            var vm = new LoginPageViewModel { ActiveTab = "login" };
            ViewData["GoogleEnabled"] = _googleSettings.Enabled;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                vm.ErrorMessage = "Please enter your email and password.";
                return View(vm);
            }

            var user = await _userManager.FindByEmailAsync(email) ?? await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                vm.ErrorMessage = "Email hoặc mật khẩu không đúng.";
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, password, rememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Check if user is blocked by admin
                if (user.IsBlocked)
                {
                    await _signInManager.SignOutAsync();
                    vm.ErrorMessage = $"Tài khoản của bạn đã bị khóa. Lý do: {user.BlockedReason ?? "Vi phạm chính sách cộng đồng."}";
                    return View(vm);
                }
                return RedirectToAction("Index", "Home");
            }

            vm.ErrorMessage = "Email hoặc mật khẩu không đúng.";
            return View(vm);
        }

        // POST /Account/Register
        // Read form values directly — avoids Login fields failing validation
        [HttpPost, ActionName("Register"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost()
        {
            var firstName       = Request.Form["Register.FirstName"].ToString().Trim();
            var lastName        = Request.Form["Register.LastName"].ToString().Trim();
            var username        = Request.Form["Register.Username"].ToString().Trim();
            var email           = Request.Form["Register.Email"].ToString().Trim();
            var password        = Request.Form["Register.Password"].ToString();
            var confirmPassword = Request.Form["Register.ConfirmPassword"].ToString();

            var vm = new LoginPageViewModel { ActiveTab = "register" };
            ViewData["GoogleEnabled"] = _googleSettings.Enabled;

            // Basic validation
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                vm.ErrorMessage = "Please fill in all required fields.";
                return View("Login", vm);
            }

            if (password != confirmPassword)
            {
                vm.ErrorMessage = "Passwords do not match.";
                return View("Login", vm);
            }

            if (password.Length < 8)
            {
                vm.ErrorMessage = "Password must be at least 8 characters.";
                return View("Login", vm);
            }

            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName  = lastName,
                UserName  = email,
                Email     = email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            vm.ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            return View("Login", vm);
        }

        // GET /Account/GoogleLogin
        [HttpGet]
        public IActionResult GoogleLogin()
        {
            if (!_googleSettings.Enabled)
            {
                TempData["Error"] = "Google sign-in is not configured.";
                return RedirectToAction("Login");
            }

            var redirectUrl = Url.Action("GoogleCallback", "Account", null, Request.Scheme);
            var properties  = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        // GET /Account/GoogleCallback
        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string? remoteError = null)
        {
            if (remoteError != null)
            {
                TempData["Error"] = $"Google error: {remoteError}";
                return RedirectToAction("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Error"] = "Could not load Google info. Please try again.";
                return RedirectToAction("Login");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user?.IsBlocked == true)
                {
                    await _signInManager.SignOutAsync();
                    TempData["Error"] = $"Tài khoản của bạn đã bị khóa. Lý do: {user.BlockedReason ?? "Vi phạm chính sách cộng đồng."}";
                    return RedirectToAction("Login");
                }
                return RedirectToAction("Index", "Home");
            }

            var email     = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "User";
            var lastName  = info.Principal.FindFirstValue(ClaimTypes.Surname)   ?? "";
            var picture   = info.Principal.Claims
                                .FirstOrDefault(c => c.Type.Contains("picture"))?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Could not get email from Google.";
                return RedirectToAction("Login");
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                if (existingUser.IsBlocked)
                {
                    TempData["Error"] = $"Tài khoản của bạn đã bị khóa. Lý do: {existingUser.BlockedReason ?? "Vi phạm chính sách cộng đồng."}";
                    return RedirectToAction("Login");
                }
                await _userManager.AddLoginAsync(existingUser, info);
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            var newUser = new ApplicationUser
            {
                UserName          = email,
                Email             = email,
                FirstName         = firstName,
                LastName          = lastName,
                ProfilePictureUrl = picture,
                EmailConfirmed    = true,
                CreatedAt         = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                TempData["Error"] = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return RedirectToAction("Login");
            }

            await _userManager.AddLoginAsync(newUser, info);
            await _signInManager.SignInAsync(newUser, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        // POST /Account/Logout
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // POST /Account/ChangePassword
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var hasPassword = await _userManager.HasPasswordAsync(user);

            IdentityResult result;
            if (!hasPassword)
            {
                // Tài khoản đăng nhập bằng Google chưa có mật khẩu — dùng AddPasswordAsync
                result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            }
            else
            {
                result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            }

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return Json(new { success = true, message = "Password updated successfully!" });
            }

            return Json(new { success = false, message = string.Join(" ", result.Errors.Select(e => e.Description)) });
        }
    }
}
