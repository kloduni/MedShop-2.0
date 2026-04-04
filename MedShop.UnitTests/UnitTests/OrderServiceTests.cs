using MedShop.Core.Contracts;
using MedShop.Core.Services;
using MedShop.Infrastructure.Data;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private ApplicationDbContext dbContext;
        private IApplicationDbContext context;
        private IOrderService orderService;

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
        public async Task TestStoreOrder_StoresCorrectValues()
        {
            orderService = new OrderService(context);

            var product = new Product() { Id = 1, ProductName = "p1", Price = 10, Description = "", ImageUrl = "" };
            var user = new User() { Id = "u1", Email = "u@m.com", UserName = "u" };

            await context.Products.AddAsync(product);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var items = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem() { Product = product, Amount = 1, ShoppingCartId = "c1" }
            };

            await orderService.StoreOrderAsync(items, user.Id, user.Email);

            var orderItem = await context.OrderItems.FirstOrDefaultAsync();
            Assert.IsNotNull(orderItem);
            Assert.That(orderItem.ProductId, Is.EqualTo(1));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}