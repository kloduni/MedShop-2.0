using MedShop.Core.Contracts;
using MedShop.Core.Models.Admin;
using MedShop.Core.Models.User;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Core.Services
{
    public class UserStatisticsService : IUserStatisticsService
    {
        private readonly IApplicationDbContext context;

        public UserStatisticsService(IApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<StatisticsViewModel> UsersInfo()
        {
            int totalUsers = await context.Users.AsNoTracking()
                .CountAsync(u => u.Id != null);
            int activeUsers = await context.Users.AsNoTracking()
                .CountAsync(u => u.IsActive);
            int totalProducts = await context.Products.AsNoTracking()
                .CountAsync(p => p.Id != -1);
            int activeProducts = await context.Products.AsNoTracking()
                .CountAsync(p => p.IsActive);

            return new StatisticsViewModel()
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalProducts = totalProducts,
                ActiveProducts = activeProducts
            };
        }

        public async Task<IEnumerable<CategoryStatModel>> GetProductsByCategory()
        {
            return await context.Products.AsNoTracking()
                .Where(p => p.IsActive) // Only count active products
                .GroupBy(p => p.Category.Name) // Group by category name
                .Select(g => new CategoryStatModel
                {
                    Category = g.Key ?? "Uncategorized",
                    Count = g.Count()
                })
                .ToListAsync();
        }
    }
}
