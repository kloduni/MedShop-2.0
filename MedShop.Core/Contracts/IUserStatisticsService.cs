using MedShop.Core.Models.Admin;
using MedShop.Core.Models.User;

namespace MedShop.Core.Contracts
{
    public interface IUserStatisticsService
    {
        Task<StatisticsViewModel> UsersInfo();
        Task<IEnumerable<CategoryStatModel>> GetProductsByCategory();
    }
}
