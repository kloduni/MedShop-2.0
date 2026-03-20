using MedShop.Core.Models.Order;
using MedShop.Infrastructure.Data.Models;

namespace MedShop.Core.Contracts
{
    public interface IOrderService
    {
        Task StoreOrderAsync(ICollection<ShoppingCartItem> items, string userId, string userEmailAddress);
        Task<ICollection<OrderServiceModel>> GetOrdersByUserIdAsync(string userId);
    }
}
