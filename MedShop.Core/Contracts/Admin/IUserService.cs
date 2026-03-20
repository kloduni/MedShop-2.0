using MedShop.Core.Models.Admin;
using MedShop.Infrastructure.Data.Models;

namespace MedShop.Core.Contracts.Admin
{
    public interface IUserService
    {
        Task<IEnumerable<UserServiceModel>> All();
        Task BanUserAsync(User user);
        Task UnbanUserAsync(User user);
    }
}
