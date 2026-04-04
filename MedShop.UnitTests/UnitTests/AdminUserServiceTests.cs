using MedShop.Core.Contracts;
using MedShop.Core.Contracts.Admin;
using MedShop.Core.Data.Models;
using MedShop.Core.Services.Admin;
using MedShop.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class AdminUserServiceTests
    {
        private ApplicationDbContext dbContext;
        private IApplicationDbContext context;
        private IUserService userService;

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
        public async Task TestAll_ReturnsCorrectValues()
        {
            userService = new UserService(context);

            var baseModel = await userService.All();
            var baseUsersCount = baseModel.Count();

            var user1 = new User() { Id = "test-u99", Email = "u1@m.com", UserName = "u1", IsActive = true };
            var user2 = new User() { Id = "test-u100", Email = "u2@m.com", UserName = "u2", IsActive = true };
            await context.Users.AddRangeAsync(user1, user2);
            await context.SaveChangesAsync();

            var model = await userService.All();

            Assert.That(model.Count(), Is.EqualTo(baseUsersCount + 2));

            Assert.IsTrue(model.Any(u => u.UserId == "test-u99"));
            Assert.IsTrue(model.Any(u => u.UserId == "test-u100"));
        }

        [Test]
        public async Task TestBanUser_ChangesActiveStatusCorrectly()
        {
            userService = new UserService(context);

            await context.Users.AddAsync(new User() { Id = "test-ban-99", IsActive = true, UserName = "u2" });
            await context.SaveChangesAsync();

            await userService.BanUserAsync("test-ban-99");

            var bannedUser = await context.Users.FindAsync("test-ban-99");
            Assert.IsNotNull(bannedUser);
            Assert.IsFalse(bannedUser.IsActive);
        }

        [Test]
        public async Task TestUnbanUser_ChangesActiveStatusCorrectly()
        {
            userService = new UserService(context);

            await context.Users.AddAsync(new User() { Id = "test-unban-99", IsActive = false, UserName = "u2" });
            await context.SaveChangesAsync();

            await userService.UnbanUserAsync("test-unban-99");

            var unbannedUser = await context.Users.FindAsync("test-unban-99");
            Assert.IsNotNull(unbannedUser);
            Assert.IsTrue(unbannedUser.IsActive);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}