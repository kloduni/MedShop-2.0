using MedShop.Core.Contracts;
using MedShop.Core.Models.User;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
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

        /// <summary>
        /// Gets user and product info
        /// </summary>
        /// <returns></returns>
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
    }
}
