using MedShop.Core.Contracts.Admin;
using MedShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static MedShop.Areas.Admin.AdminConstants;
using static MedShop.Core.Constants.MessageConstants;

namespace MedShop.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;

        public UserController(IUserService _userService, UserManager<User> _userManager)
        {
            userService = _userService;
            userManager = _userManager;
        }
        public async Task<IActionResult> All()
        {
            var model = await userService.All();

            return View(model);
        }

        public async Task<IActionResult> Ban(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData[ErrorMessage] = UserNotFound;
                return RedirectToAction(nameof(All));

            }

            if (await userManager.IsInRoleAsync(user, "Administrator"))
            {
                TempData[ErrorMessage] = UserIsAdmin;
                return RedirectToAction(nameof(All));
            }

            if (user.IsActive)
            {
                TempData[SuccessMessage] = UserBanned;
                await userService.BanUserAsync(user);
            }
            else
            {
                TempData[WarningMessage] = UserAlreadyBanned;
            }

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Unban(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData[ErrorMessage] = UserNotFound;
                return RedirectToAction(nameof(All));

            }

            if (user.IsActive)
            {
                TempData[ErrorMessage] = UserNotBanned;

            }
            else
            {
                await userService.UnbanUserAsync(user);
                TempData[SuccessMessage] = UserUnbanned;
            }

            return RedirectToAction(nameof(All));
        }
    }
}
