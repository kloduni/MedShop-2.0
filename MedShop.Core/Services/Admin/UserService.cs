using MedShop.Core.Contracts;
using MedShop.Core.Contracts.Admin;
using MedShop.Core.Models.Admin;
using MedShop.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static MedShop.Core.Constants.User.AdminConstants;

namespace MedShop.Core.Services.Admin
{
    public class UserService : IUserService
    {
        private readonly IRepository repo;


        public UserService(IRepository _repo)
        {
            repo = _repo;
        }


        public async Task<IEnumerable<UserServiceModel>> All()
        {
            // Resolve the Administrator role ID so we can exclude admins from the list.
            // Admins are managed separately and should never appear in the user management UI.
            var adminRole = await repo.AllReadonly<IdentityRole>()
                .FirstOrDefaultAsync(r => r.Name == AdminRoleName);

            var adminRoleId = adminRole?.Id ?? string.Empty;

            var adminUserIds = await repo.AllReadonly<IdentityUserRole<string>>()
                .Where(ur => ur.RoleId == adminRoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            return await repo.AllReadonly<User>()
                .Where(u => !adminUserIds.Contains(u.Id))
                .Select(u => new UserServiceModel()
                {
                    UserId = u.Id,
                    Email = u.Email ?? string.Empty,
                    UserName = u.UserName ?? string.Empty,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task BanUserAsync(User user)
        {
            user.IsActive = false;

            await repo.SaveChangesAsync();
        }

        public async Task UnbanUserAsync(User user)
        {
            user.IsActive = true;

            await repo.SaveChangesAsync();
        }
    }
}
