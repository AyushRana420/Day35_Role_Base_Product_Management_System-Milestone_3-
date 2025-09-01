using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Role_Base_Product_Management_System.Models;
using System.Threading.Tasks;

namespace Role_Base_Product_Management_System.Controllers {
    public class AccountController : Controller {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                if (!string.IsNullOrEmpty(model.Role) && (model.Role == "Admin" || model.Role == "Manager")) {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null) {
            if (!ModelState.IsValid) return View(model);

            var res = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (res.Succeeded) {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
