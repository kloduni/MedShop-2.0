using MedShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using static MedShop.Core.Constants.User.AdminConstants;

namespace MedShop.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedAdminAsync(this IApplicationBuilder app)
        {
            // Create a scope to get our scoped Identity services
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            
            string adminRoleName = AdminRoleName;
            string adminEmail = AdminEmail;

            // 1. Check if the Admin role exists, create it if not
            if (await roleManager.RoleExistsAsync(adminRoleName) == false)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            // 2. Find the Admin user
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