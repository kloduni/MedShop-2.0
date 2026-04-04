using MedShop.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using static MedShop.Core.Constants.User.AdminConstants;

namespace MedShop.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Ensures the Administrator role exists and the configured admin user is assigned to it.
        /// Called once at startup; subsequent calls are safe no-ops because existence checks are
        /// performed before any create/assign operations.
        /// </summary>
        public static async Task SeedAdminAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string adminRoleName = AdminRoleName;
            string adminEmail = AdminEmail;

            if (await roleManager.RoleExistsAsync(adminRoleName) == false)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
            {
                if (await userManager.IsInRoleAsync(adminUser, adminRoleName) == false)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                }
            }
        }
    }
}