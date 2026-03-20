using MedShop.Core.Contracts;
using MedShop.Core.Services;
using MedShop.Infrastructure.Data;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private IRepository repo;
        private IOrderService orderService;
        private ApplicationDbContext context;

        [SetUp]
        public void SetUp()
        {
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("MedShopTestDb")
                .Options;

            context = new ApplicationDbContext(contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Test]
        public async Task TestStoreOrder_StoresCorrectValues()
        {
            var tRepo = new Repository(context);
            orderService = new OrderService(tRepo);

            var items = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem()
                {
                    Id = 1,
                    Amount = 1,
                    ShoppingCartId = "9cf147b8-412b-455b-b587-68ae68606cf1",
                    Product = new Product()
                    {
                        Id = 1,
                        ProductName = "product1",
                        Description = "",
                        ImageUrl = "",
                        Quantity = 10
                    }
                }
            };

            var user = new User()
            {
                Id = "3a45d2af-9dfa-4c52-87b8-780a0374b8ab",
                Email = "user@medshop.com",
                EmailConfirmed = true,
                IsActive = true,
                UserName = "user"
            };

            await tRepo.AddAsync(user);
            await tRepo.SaveChangesAsync();

            await orderService.StoreOrderAsync(items, user.Id, user.Email);

            var order = await tRepo.AllReadonly<Order>()
                .Include(o => o.User)
                .FirstOrDefaultAsync();
            var orderItem = await tRepo.AllReadonly<OrderItem>().FirstOrDefaultAsync();

            Assert.IsNotNull(order);
            Assert.IsNotNull(orderItem);
            Assert.That(orderItem.OrderId, Is.EqualTo(order.Id));
            Assert.That(order.UserId, Is.EqualTo("3a45d2af-9dfa-4c52-87b8-780a0374b8ab"));
            Assert.That(orderItem.ProductId, Is.EqualTo(1));
        }

        [Test]
        public async Task TestGetOrdersByUserId_ReturnsCorrectOrders()
        {
            var tRepo = new Repository(context);
            orderService = new OrderService(tRepo);

            var user = new User()
            {
                Id = "3a45d2af-9dfa-4c52-87b8-780a0374b8ab",
                UserName = "user",
                Email = "user@medshop.com",
                EmailConfirmed = true,
                IsActive = true
            };

            await tRepo.AddAsync(user);
            await tRepo.SaveChangesAsync();


            var orders = new List<Order>()
            {
                new Order()
                {
                    Id = 1,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Id = 1,
                            Price = 10,
                            Amount = 10,
                            Product = new Product()
                            {
                                Id = 1,
                                ProductName = "product",
                                UsersProducts = new List<UserProduct>()
                                {
                                    new UserProduct()
                                    {
                                        User = user,
                                        ProductId = 1
                                    }
                                },
                                Description = "",
                                ImageUrl = "",
                                IsActive = true
                            }
                        },
                        new OrderItem()
                        {
                            Id = 2,
                            Price = 20,
                            Amount = 20,
                            Product = new Product()
                            {
                                Id = 2,
                                ProductName = "product2",
                                UsersProducts = new List<UserProduct>()
                                {
                                    new UserProduct()
                                    {
                                        User = user,
                                        ProductId = 2
                                    }
                                },
                                Description = "",
                                ImageUrl = "",
                                IsActive = true
                            }
                        }
                    },
                    UserId = user.Id,
                    Email = user.Email,
                    User = user
                }
            };

            await tRepo.AddRangeAsync(orders);
            await tRepo.SaveChangesAsync();

            var userOrders = await orderService.GetOrdersByUserIdAsync("3a45d2af-9dfa-4c52-87b8-780a0374b8ab");

            Assert.IsNotNull(userOrders);
            Assert.That(userOrders.Count, Is.EqualTo(1));

            var userOrder = userOrders.First();

            Assert.That(userOrder.Id, Is.EqualTo(1));
            Assert.That(userOrder.TotalPrice, Is.EqualTo("500.00"));
            Assert.That(userOrder.UserName, Is.EqualTo("user"));
            Assert.That(userOrder.OrderItems.First().Id, Is.EqualTo(1));
            Assert.That(userOrder.OrderItems.First().Amount, Is.EqualTo(10));
            Assert.That(userOrder.OrderItems.First().Price, Is.EqualTo(10));
            Assert.That(userOrder.OrderItems.Reverse().First().Id, Is.EqualTo(2));
            Assert.That(userOrder.OrderItems.Reverse().First().Amount, Is.EqualTo(20));
            Assert.That(userOrder.OrderItems.Reverse().First().Price, Is.EqualTo(20));
        }


        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
