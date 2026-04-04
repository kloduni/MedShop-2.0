using MedShop.Core.Contracts;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MedShop.Core.Cart
{
    public class ShoppingCart
    {
        private readonly IApplicationDbContext context;

        public ShoppingCart(IApplicationDbContext _context)
        {
            context = _context;
        }

        public string ShoppingCartId { get; set; }

        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        /// <summary>
        /// Retrieves the current user's shopping cart from the session, creating one with a new
        /// GUID if no cart ID exists yet.  The GUID is persisted in the session so the same
        /// cart is reused for the duration of the browser session.
        /// </summary>
        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var repo = services.GetRequiredService<IApplicationDbContext>();
            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId);

            return new ShoppingCart(repo)
            {
                ShoppingCartId = cartId
            };
        }

        public async Task AddItemToCartAsync(Product product)
        {
            var shoppingCartItem = await context.ShoppingCartItems.FirstOrDefaultAsync(i => i.Product.Id == product.Id && i.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = ShoppingCartId,
                    Product = product,
                    Amount = 1
                };

                await context.ShoppingCartItems.AddAsync(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }

            await context.SaveChangesAsync();
        }

        public async Task RemoveItemFromCartAsync(ShoppingCartItem cartItem)
        {
            var shoppingCartItem = await context.ShoppingCartItems
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
                    context.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            await context.SaveChangesAsync();
        }

        public ICollection<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = context.ShoppingCartItems
                .Where(n => n.ShoppingCartId == ShoppingCartId)
                .Include(n => n.Product)
                .ToList());
        }

        public async Task<ShoppingCartItem> GetCartItemByIdAsync(int cartItemId)
        {
            return await context.ShoppingCartItems.AsNoTracking()
                .Where(i => i.Id == cartItemId)
                .FirstOrDefaultAsync();
        }

        public async Task<double> GetShoppingCartTotalAsync() => await context.ShoppingCartItems
            .Where(n => n.ShoppingCartId == ShoppingCartId)
            .Select(n => (double)(n.Product.Price * n.Amount))
            .SumAsync();

        public async Task ClearShoppingCartAsync()
        {
            var items = await context.ShoppingCartItems
                .Where(n => n.ShoppingCartId == ShoppingCartId)
                .ToListAsync();

            context.ShoppingCartItems.RemoveRange(items);

            await context.SaveChangesAsync();
        }
    }
}
