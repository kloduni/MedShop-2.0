using MedShop.Core.Models.Admin;
using MedShop.Core.Data.Models;

namespace MedShop.Core.Contracts.Admin
{
    public interface IUserService
    {
        Task<IEnumerable<UserServiceModel>> All();
        Task BanUserAsync(User user);
        Task UnbanUserAsync(User user);
    }
}
