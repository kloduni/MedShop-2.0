using MedShop.Core.Models.User;

namespace MedShop.Core.Contracts
{
    public interface IUserStatisticsService
    {
        Task<StatisticsViewModel> UsersInfo();
    }
}
