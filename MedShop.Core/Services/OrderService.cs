using MedShop.Core.Contracts;
using MedShop.Core.Models.Order;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository repo;

        public OrderService(IRepository _repo)
        {
            repo = _repo;
        }

        /// <summary>
        /// Stores user order with items in Db
        /// </summary>
        /// <param name="items"></param>
        /// <param name="userId"></param>
        /// <param name="userEmailAddress"></param>
        /// <returns></returns>
        public async Task StoreOrderAsync(ICollection<ShoppingCartItem> items, string userId, string userEmailAddress)
        {
            var order = new Order()
            {
                UserId = userId,
                Email = userEmailAddress
            };

            await repo.AddAsync(order);
            await repo.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem()
                {
                    Amount = item.Amount,
                    ProductId = item.Product.Id,
                    OrderId = order.Id,
                    Price = item.Product.Price
                };
                await repo.AddAsync(orderItem);
            }

            await repo.SaveChangesAsync();
        }

        /// <summary>
        /// Gets orders by user Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ICollection<OrderServiceModel>> GetOrdersByUserIdAsync(string userId)
        {
            return await repo.All<Order>()
                .Where(o => o.User.Id == userId)
                .Select(o => new OrderServiceModel()
                {
                    Id = o.Id,
                    OrderItems = repo.All<OrderItem>()
                        .Where(oi => oi.Order.Id == o.Id)
                        .Select(oi => new OrderItemServiceModel()
                        {
                            Id = oi.Id,
                            Price = oi.Price,
                            Amount = oi.Amount,
                            ProductName = oi.Product.ProductName
                        })
                        .ToList(),
                    UserName = o.OrderItems.Select(oi => oi.Product.UsersProducts.Select(up => up.User.UserName).First()).First(),
                    TotalPrice = o.OrderItems.Sum(oi => oi.Price * oi.Amount).ToString("f2")
                })
                .ToListAsync();
        }
    }
}
