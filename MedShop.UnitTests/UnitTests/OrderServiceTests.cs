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

            var category = new Category { Id = 99, Name = "TestCategory" };
            var product = new Product() { Id = 99, CategoryId = 99, ProductName = "p1", Price = 10, Description = "desc", ImageUrl = "img" };
            var user = new User() { Id = "test-u99", Email = "u@m.com", UserName = "u" };

            await context.Categories.AddAsync(category);
            await context.Products.AddAsync(product);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var items = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem() { Id = 99, Product = product, Amount = 1, ShoppingCartId = "test-cart-99" }
            };

            await orderService.StoreOrderAsync(items, user.Id, user.Email);

            var order = await context.Orders.FirstOrDefaultAsync(o => o.UserId == "test-u99");
            var orderItem = await context.OrderItems.FirstOrDefaultAsync(oi => oi.ProductId == 99);

            Assert.IsNotNull(order);
            Assert.IsNotNull(orderItem);
            Assert.That(orderItem.ProductId, Is.EqualTo(99));
        }

        [Test]
        public async Task TestGetOrdersByUserId_ReturnsCorrectOrders()
        {
            orderService = new OrderService(context);

            var seller = new User() { Id = "test-seller", UserName = "seller" };
            var buyer = new User() { Id = "test-buyer", UserName = "buyer", Email = "b@m.com" };
            var category = new Category { Id = 99, Name = "TestCategory" };
            var product = new Product() { Id = 99, CategoryId = 99, ProductName = "product", Price = 50, Description = "desc", ImageUrl = "img", IsActive = true };

            await context.Users.AddRangeAsync(seller, buyer);
            await context.Categories.AddAsync(category);
            await context.Products.AddAsync(product);

            await context.UsersProducts.AddAsync(new UserProduct { UserId = seller.Id, ProductId = 99 });

            var order = new Order()
            {
                Id = 99,
                UserId = buyer.Id,
                Email = buyer.Email,
                OrderItems = new List<OrderItem>()
                {
                    new OrderItem() { Id = 99, Price = 50, Amount = 2, ProductId = 99 }
                }
            };
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            var userOrders = await orderService.GetOrdersByUserIdAsync(buyer.Id);

            Assert.IsNotNull(userOrders);
            Assert.That(userOrders.Count, Is.EqualTo(1));

            var userOrder = userOrders.First();
            Assert.That(userOrder.TotalPrice, Is.EqualTo("100.00"));
            Assert.That(userOrder.UserName, Is.EqualTo("seller"));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}