using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BankMvc.Models.Entity;
using System.Security.Claims;
using BankMvc.Contract.Service;
using BankMvc.DTO.ViewModels;
using Bank.Services;
using BankMvc.Contract.Repository;
using Microsoft.AspNetCore.Identity;
using BankMvc.Service;

namespace BankMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;

        public AuthController(IAuthService authService, IAccountService accountService)
        {
            _authService = authService;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If user is already logged in, redirect to dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Account");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add logging to see what's happening
                    Console.WriteLine($"Attempting to authenticate user: {model.Username}");

                    var user = await _authService.AuthenticateAsync(model.Username, model.Password);

                    if (user != null)
                    {
                        Console.WriteLine($"User authenticated successfully: {user.Username}");

                        // Sign in the user
                        await SignInUserAsync(user, model.RememberMe);

                        // Set success message
                        TempData["SuccessMessage"] = $"Welcome back, {user.FirstName}!";

                        // Redirect to dashboard or return URL
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Dashboard", "Account");
                    }
                    else
                    {
                        Console.WriteLine("Authentication failed - user is null");
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the specific error details
                    Console.WriteLine($"Login error: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    // More specific error handling
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    ModelState.AddModelError("", $"An error occurred during login: {ex.Message}");
                }
            }
            else
            {
                // Log model state errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model validation error: {error.ErrorMessage}");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            // If user is already logged in, redirect to dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username already exists  
                    if (await _authService.UsernameExistsAsync(model.Username))
                    {
                        ModelState.AddModelError("Username", "Username already exists.");
                        return View(model);
                    }

                    // Check if email already exists  
                    if (await _authService.EmailExistsAsync(model.Email))
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                        return View(model);
                    }

                    // Create new user  
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Username = model.Username,
                        Email = model.Email,
                        CreatedAt = DateTime.UtcNow,
                        LastLoginDate = DateTime.UtcNow
                    };

                    // Create user and sign them in
                    await _authService.CreateUserAsync(user, model.Password);
                    var result = AuthResult.Success("User created successfully", user);

                    return RedirectToAction("Login");
              
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                    // Log the exception  
                }
            }

            return View(model);
        }

        // ADD THIS: Logout functionality
        //[HttpPost]
        //public async Task<IActionResult> Logout()
        //{
        //    try
        //    {
        //        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //        TempData["SuccessMessage"] = "You have been successfully logged out.";
        //        return RedirectToAction("Index", "Home");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        TempData["ErrorMessage"] = "An error occurred during logout.";
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        // ADD THIS: GET version of logout for direct URL access
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["SuccessMessage"] = "You have been successfully logged out.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["ErrorMessage"] = "An error occurred during logout.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Username availability check
        [HttpPost]
        public async Task<IActionResult> CheckUsernameAvailability(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                {
                    return Json(new { available = false, message = "Username must be at least 3 characters long." });
                }

                bool exists = await _authService.UsernameExistsAsync(username);

                if (exists)
                {
                    return Json(new { available = false, message = "Username is already taken." });
                }
                else
                {
                    return Json(new { available = true, message = "Username is available!" });
                }
            }
            catch (Exception)
            {
                return Json(new { available = false, message = "Error checking username availability." });
            }
        }

        // Helper method for signing in users
        private async Task SignInUserAsync(User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            // Update last login date
            await _authService.UpdateLastLoginAsync(user.UserId);
        }
    }
}