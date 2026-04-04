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
            await context.Users.AddAsync(new User() { Id = "1", IsActive = true });
            await context.SaveChangesAsync();

            var result = await userStatisticsService.UsersInfo();
            Assert.That(result.TotalUsers, Is.EqualTo(1));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}