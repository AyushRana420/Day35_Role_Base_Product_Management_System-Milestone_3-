using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Role_Base_Product_Management_System.Models;
using System.Threading.Tasks;

namespace Role_Base_Product_Management_System.Controllers {
    public class AccountController : Controller {
        private readonly UserManager<ApplicationUser> _userManager;//Using UserManager to manage user accounts
        private readonly SignInManager<ApplicationUser> _signInManager;//Using SignInManager to manage user sign-in

        //Constructor for AccountController
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() => View();//Display the registration form

        [HttpPost]
        [ValidateAntiForgeryToken]//Validate the form submission
        public async Task<IActionResult> Register(RegisterViewModel model)//Method for user registration
        {
            if (!ModelState.IsValid) return View(model);//If the model state is invalid, return the view with the model

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = true };//Create a new user
            var result = await _userManager.CreateAsync(user, model.Password);//Create the user

            if (result.Succeeded)//If user creation is successful
            {
                if (!string.IsNullOrEmpty(model.Role) && (model.Role == "Admin" || model.Role == "Manager"))//Check if the role is valid
                {
                    await _userManager.AddToRoleAsync(user, model.Role);//Assign the role to the user
                }
                TempData["Success"] = "Registration successful! Please login.";//Set success message
                return RedirectToAction("Login", "Account");//Redirect to login
            }

            foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);//Returns error messages
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) { //Display the login form
            ViewData["ReturnUrl"] = returnUrl; //Store the return URL
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null) { //Handle login form submission
            if (!ModelState.IsValid) return View(model);//If the model state is invalid, return the view with the model

            var res = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false); //Attempt to sign in the user
            if (res.Succeeded) {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);//If signed in successfully and returnUrl is local, redirect to returnUrl
                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
