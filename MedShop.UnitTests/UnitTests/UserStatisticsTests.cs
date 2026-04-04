using MedShop.Core.Contracts;
using MedShop.Core.Services;
using MedShop.Infrastructure.Data;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class UserStatisticsTests
    {
        private ApplicationDbContext dbContext;
        private IApplicationDbContext context;
        private IUserStatisticsService userStatisticsService;

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
        public async Task TestStatisticsUsersInfo_ReturnsCorrectValues()
        {
            userStatisticsService = new UserStatisticsService(context);

            // Dynamically capture the base counts (to ignore any seeded data)
            var baseTotalUsers = await context.Users.CountAsync();
            var baseActiveUsers = await context.Users.CountAsync(u => u.IsActive);
            var baseTotalProducts = await context.Products.CountAsync();
            var baseActiveProducts = await context.Products.CountAsync(p => p.IsActive);

            var category = new Category { Id = 99, Name = "TestCategory" };
            await context.Categories.AddAsync(category);

            await context.Users.AddRangeAsync(new List<User>()
            {
                new User() { Id = "test-u99", IsActive = true, UserName = "u1" },
                new User() { Id = "test-u100", IsActive = false, UserName = "u2" }
            });

            await context.Products.AddRangeAsync(new List<Product>()
            {
                new Product() { Id = 99, CategoryId = 99, IsActive = true, ProductName = "p1", Description = "desc", ImageUrl = "img" },
                new Product() { Id = 100, CategoryId = 99, IsActive = false, ProductName = "p2", Description = "desc", ImageUrl = "img" }
            });
            await context.SaveChangesAsync();

            var model = await userStatisticsService.UsersInfo();

            // Assert based on the offset of the data we just explicitly added
            Assert.That(model.TotalUsers, Is.EqualTo(baseTotalUsers + 2));
            Assert.That(model.ActiveUsers, Is.EqualTo(baseActiveUsers + 1));
            Assert.That(model.TotalProducts, Is.EqualTo(baseTotalProducts + 2));
            Assert.That(model.ActiveProducts, Is.EqualTo(baseActiveProducts + 1));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}