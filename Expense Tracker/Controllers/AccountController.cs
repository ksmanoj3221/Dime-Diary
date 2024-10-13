using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Expense_Tracker.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Expense_Tracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Accounts/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(Registration registration)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = registration.UserName, Email = registration.Email };
                var result = await _userManager.CreateAsync(user, registration.Password);

                if (result.Succeeded)
                {
                    // Optionally, sign in the user or redirect to a confirmation page
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View("~/Views/Accounts/Register.cshtml", registration);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Accounts/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                // Fetch the user by email
                var user = await _userManager.FindByEmailAsync(login.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("~/Views/Accounts/Login.cshtml", login); // Return login view on invalid email
                }

                // Use the user's username for login (ASP.NET Identity needs username for authentication)
                var result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in: {Email}", login.Email);
                    return RedirectToAction("Index", "Dashboard"); // Redirect to Dashboard on successful login
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we reach here, return the login view with the login model and any validation errors
            return View("~/Views/Accounts/Login.cshtml", login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // Redirect to the login page after logout
            return RedirectToAction("Login", "Account");
        }

    }
}
