using MedShop.Core.Cart;
using MedShop.Core.Contracts;
using MedShop.Infrastructure.Data;
using MedShop.Core.Data.Models;
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
            shoppingCart.ShoppingCartId = "test-cart";
            var product = new Product() { Id = 1, ProductName = "p", Price = 10 };

            await shoppingCart.AddItemToCartAsync(product);
            Assert.That(await context.ShoppingCartItems.CountAsync(), Is.EqualTo(1));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}