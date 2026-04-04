using MedShop.Core.Contracts;
using MedShop.Core.Contracts.Admin;
using MedShop.Core.Models.Admin;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using static MedShop.Core.Constants.User.AdminConstants;

namespace MedShop.Core.Services.Admin
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext context;


        public UserService(IApplicationDbContext _context)
        {
            context = _context;
        }


        public async Task<IEnumerable<UserServiceModel>> All()
        {
            // Resolve the Administrator role ID so we can exclude admins from the list.
            // Admins are managed separately and should never appear in the user management UI.
            var adminRole = await context.Roles.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == AdminRoleName);

            var adminRoleId = adminRole?.Id ?? string.Empty;

            var adminUserIds = await context.UserRoles.AsNoTracking()
                .Where(ur => ur.RoleId == adminRoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            return await context.Users.AsNoTracking()
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

        public async Task BanUserAsync(string userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task UnbanUserAsync(string userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = true;
                await context.SaveChangesAsync();
            }
        }
    }
}
