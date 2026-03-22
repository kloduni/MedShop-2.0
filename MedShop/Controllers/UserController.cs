using MedShop.Core.Models.User;
using MedShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static MedShop.Core.Constants.MessageConstants;
using static MedShop.Core.Constants.User.UserConstants;
using static MedShop.Core.Constants.User.AdminConstants;

namespace MedShop.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private IMemoryCache cache;

        public UserController(UserManager<User> _userManager, SignInManager<User> _signInManager, RoleManager<IdentityRole> _roleManager, IMemoryCache _cache)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            cache = _cache;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "User");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            if (result.Errors.Any())
            {
                TempData[ErrorMessage] = result.Errors.First().Description;
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                if (!user.IsActive)
                {
                    TempData[ErrorMessage] = Banned;

                    return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
                }

                var result = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (HttpContext.Session.GetString("UserId") == null)
                    {
                        HttpContext.Session.SetString("UserId", user.Id);
                    }

                    if(HttpContext.Session.GetString("UserId") != user.Id)
                    {
                        HttpContext.Session.SetString("CartId", Guid.NewGuid().ToString());
                        HttpContext.Session.SetString("UserId", user.Id);
                    }

                    if (await userManager.IsInRoleAsync(user, AdminRoleName))
                    {
                        return RedirectToAction("Index", "Home", new { area = AreaName });
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            TempData[ErrorMessage] = "Invalid Login!";
            ModelState.AddModelError("", "Invalid Login!");

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData[ErrorMessage] = UserNotFound;
                return RedirectToAction("Index", "Home");
            }

            var model = new ManageViewModel
            {
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Manage(ManageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData[ErrorMessage] = UserNotFound;
                return RedirectToAction("Index", "Home");
            }

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                var emailResult = await userManager.UpdateAsync(user);

                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError(string.Empty, "Current password is required to set a new password.");
                    return View(model);
                }

                var passwordResult = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                await signInManager.RefreshSignInAsync(user);
                TempData[SuccessMessage] = PasswordChanged;
                return RedirectToAction(nameof(Manage));
            }

            TempData[SuccessMessage] = ProfileUpdated;
            return RedirectToAction(nameof(Manage));
        }
    }
}
