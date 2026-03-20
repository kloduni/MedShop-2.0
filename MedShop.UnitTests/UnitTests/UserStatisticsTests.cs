using MedShop.Core.Contracts;
using MedShop.Core.Services;
using MedShop.Infrastructure.Data;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class UserStatisticsTests
    {
        private IRepository repo;
        private IUserStatisticsService userStatisticsService;
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
        public async Task TestStatisticsUsersInfo_ReturnsCorrectValues()
        {
            var tRepo = new Repository(context);
            userStatisticsService = new UserStatisticsService(tRepo);

            await tRepo.AddRangeAsync(new List<User>()
            {
                new User()
                {
                    Id = "3a45d2af-9dfa-4c52-87b8-780a0374b8ab",
                    Email = "user@medshop.com",
                    EmailConfirmed = true,
                    IsActive = true,
                    UserName = "user"
                },
                new User()
                {
                    Id = "63d65a50-2c24-4943-9d64-66da5aff20b3",
                    Email = "user2@medshop.com",
                    EmailConfirmed = true,
                    IsActive = false,
                    UserName = "user2"
                }
            });

            await tRepo.AddRangeAsync(new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    Description = "",
                    ImageUrl = "",
                    ProductName = "product",
                    IsActive = true,
                },
                new Product()
                {
                    Id = 2,
                    Description = "",
                    ImageUrl = "",
                    ProductName = "product2",
                    IsActive = false,
                },
            });
            await tRepo.SaveChangesAsync();

            var model = await userStatisticsService.UsersInfo();

            Assert.That(model.TotalUsers, Is.EqualTo(2));
            Assert.That(model.TotalProducts, Is.EqualTo(2));
            Assert.That(model.ActiveUsers, Is.EqualTo(1));
            Assert.That(model.ActiveProducts, Is.EqualTo(1));
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
