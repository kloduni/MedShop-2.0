using MedShop.Core.Contracts.Admin;
using MedShop.Core.Models.Admin;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
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
            // 1. Get the Administrator Role ID
            var adminRole = await repo.AllReadonly<IdentityRole>()
                .FirstOrDefaultAsync(r => r.Name == AdminRoleName);

            var adminRoleId = adminRole?.Id ?? string.Empty;

            // 2. Look directly into the join table to get the IDs of all Admins
            var adminUserIds = await repo.AllReadonly<IdentityUserRole<string>>()
                .Where(ur => ur.RoleId == adminRoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // 3. Return all users EXCEPT those whose IDs are in the adminUserIds list
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

        /// <summary>
        /// Bans user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task BanUserAsync(User user)
        {
            user.IsActive = false;

            await repo.SaveChangesAsync();
        }

        /// <summary>
        /// Unbans user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UnbanUserAsync(User user)
        {
            user.IsActive = true;

            await repo.SaveChangesAsync();
        }
    }
}
