using MedShop.Core.Cart;
using MedShop.Core.Contracts;
using MedShop.Core.Data.Models;
using MedShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class ShoppingCartTests
    {
        private ApplicationDbContext dbContext;
        private IApplicationDbContext context;
        private ShoppingCart shoppingCart;

        [SetUp]
        public void SetUp()
        {
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            dbContext = new ApplicationDbContext(contextOptions);
            context = dbContext;

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        [Test]
        public async Task TestAddItemToCart_AddsItemToCartCorrectly()
        {
            shoppingCart = new ShoppingCart(context);
            shoppingCart.ShoppingCartId = "test-cart-99";

            var category = new Category { Id = 99, Name = "TestCategory" };
            var product = new Product() { Id = 99, CategoryId = 99, ProductName = "p", Price = 10, Description = "desc", ImageUrl = "img" };

            await context.Categories.AddAsync(category);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            await shoppingCart.AddItemToCartAsync(product);

            var item = await context.ShoppingCartItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Product.Id == 99);

            Assert.IsNotNull(item);
            Assert.That(item.Amount, Is.EqualTo(1));
        }

        [Test]
        public async Task TestRemoveItemFromCart_RemovesItemFromCartCorrectly()
        {
            shoppingCart = new ShoppingCart(context);
            shoppingCart.ShoppingCartId = "test-cart-99";

            var category = new Category { Id = 99, Name = "TestCategory" };
            var product = new Product() { Id = 99, CategoryId = 99, ProductName = "product", Price = 10, Description = "desc", ImageUrl = "img" };
            var cartItem = new ShoppingCartItem() { Id = 99, Amount = 1, ShoppingCartId = "test-cart-99", Product = product };

            await context.Categories.AddAsync(category);
            await context.ShoppingCartItems.AddAsync(cartItem);
            await context.SaveChangesAsync();

            await shoppingCart.RemoveItemFromCartAsync(cartItem);

            var remainingItems = await context.ShoppingCartItems.Where(i => i.ShoppingCartId == "test-cart-99").CountAsync();
            Assert.That(remainingItems, Is.EqualTo(0));
        }

        [Test]
        public async Task TestGetShoppingCartTotal_ReturnsCorrectSum()
        {
            shoppingCart = new ShoppingCart(context);
            shoppingCart.ShoppingCartId = "test-cart-99";

            var category = new Category { Id = 99, Name = "TestCategory" };
            var product = new Product() { Id = 99, CategoryId = 99, ProductName = "p1", Price = 10, Description = "desc", ImageUrl = "img" };
            var product2 = new Product() { Id = 100, CategoryId = 99, ProductName = "p2", Price = 20, Description = "desc", ImageUrl = "img" };

            await context.Categories.AddAsync(category);
            await context.Products.AddRangeAsync(product, product2);
            await context.SaveChangesAsync();

            await context.ShoppingCartItems.AddRangeAsync(
                new ShoppingCartItem { Id = 99, ShoppingCartId = "test-cart-99", Product = product, Amount = 2 },
                new ShoppingCartItem { Id = 100, ShoppingCartId = "test-cart-99", Product = product2, Amount = 1 }
            );
            await context.SaveChangesAsync();

            var totalSum = await shoppingCart.GetShoppingCartTotalAsync();

            Assert.That(totalSum, Is.EqualTo(40.0));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}