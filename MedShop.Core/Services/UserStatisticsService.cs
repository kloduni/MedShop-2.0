using MedShop.Core.Contracts;
using MedShop.Core.Models.Admin;
using MedShop.Core.Models.User;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Core.Services
{
    public class UserStatisticsService : IUserStatisticsService
    {
        private readonly IRepository repo;

        public UserStatisticsService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task<StatisticsViewModel> UsersInfo()
        {
            int totalUsers = await repo.AllReadonly<User>()
                .CountAsync(u => u.Id != null);
            int activeUsers = await repo.AllReadonly<User>()
                .CountAsync(u => u.IsActive);
            int totalProducts = await repo.AllReadonly<Product>()
                .CountAsync(p => p.Id != -1);
            int activeProducts = await repo.AllReadonly<Product>()
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
            return await repo.AllReadonly<Product>()
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
