using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MedShop.Core.Cart
{
    public class ShoppingCart
    {
        private readonly IRepository repo;

        public ShoppingCart(IRepository _repo)
        {
            repo = _repo;
        }

        public string ShoppingCartId { get; set; }

        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        /// <summary>
        /// Gets/Creates shopping cart with session
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var repo = services.GetService<IRepository>();
            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId);

            return new ShoppingCart(repo)
            {
                ShoppingCartId = cartId
            };
        }

        /// <summary>
        /// Adds product to shopping cart
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task AddItemToCartAsync(Product product)
        {
            var shoppingCartItem = await repo.All<ShoppingCartItem>().FirstOrDefaultAsync(i => i.Product.Id == product.Id && i.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = ShoppingCartId,
                    Product = product,
                    Amount = 1
                };

                await repo.AddAsync(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }

            await repo.SaveChangesAsync();
        }

        /// <summary>
        /// Removes product from shopping cart
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        public async Task RemoveItemFromCartAsync(ShoppingCartItem cartItem)
        {
            var shoppingCartItem = await repo.All<ShoppingCartItem>()
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == cartItem.Id && i.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                }
                else
                {
                    await repo.DeleteAsync<ShoppingCartItem>(shoppingCartItem.Id);
                }
            }
            await repo.SaveChangesAsync();
        }

        /// <summary>
        /// Gets items currently in shopping cart
        /// </summary>
        /// <returns></returns>
        public ICollection<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = repo.All<ShoppingCartItem>()
                .Where(n => n.ShoppingCartId == ShoppingCartId)
                .Include(n => n.Product)
                .ToList());
        }

        /// <summary>
        /// Gets cart item by item Id
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns></returns>
        public async Task<ShoppingCartItem> GetCartItemByIdAsync(int cartItemId)
        {
            return await repo.AllReadonly<ShoppingCartItem>()
                .Where(i => i.Id == cartItemId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets shopping cart total price
        /// </summary>
        /// <returns></returns>
        public async Task<double> GetShoppingCartTotalAsync() => await repo.All<ShoppingCartItem>()
            .Where(n => n.ShoppingCartId == ShoppingCartId)
            .Select(n => (double)(n.Product.Price * n.Amount))
            .SumAsync();

        /// <summary>
        /// Clears shopping cart items
        /// </summary>
        /// <returns></returns>
        public async Task ClearShoppingCartAsync()
        {
            var items = await repo.All<ShoppingCartItem>()
                .Where(n => n.ShoppingCartId == ShoppingCartId)
                .ToListAsync();

            repo.DeleteRange(items);

            await repo.SaveChangesAsync();
        }
    }
}
