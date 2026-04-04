using MedShop.Core.Contracts;
using MedShop.Core.Contracts.Admin;
using MedShop.Core.Services.Admin;
using MedShop.Infrastructure.Data;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class AdminUserServiceTests
    {
        private ApplicationDbContext dbContext; // Used for DB setup/disposal
        private IApplicationDbContext context;  // Used for service injection
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

            await context.Roles.AddAsync(new IdentityRole()
            {
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            });

            await context.Users.AddRangeAsync(new List<User>()
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
                    IsActive = true,
                    UserName = "user2"
                }
            });
            await context.SaveChangesAsync();

            var model = await userService.All();

            Assert.That(model.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task TestBanUser_ChangesActiveStatusCorrectly()
        {
            userService = new UserService(context);

            var user = new User() { Id = "2", IsActive = true, UserName = "u2" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await userService.BanUserAsync(user.Id);

            Assert.That(await context.Users.AsNoTracking().CountAsync(u => u.IsActive), Is.EqualTo(0));
        }

        [Test]
        public async Task TestUnbanUser_ChangesActiveStatusCorrectly()
        {
            userService = new UserService(context);

            var user = new User() { Id = "2", IsActive = false, UserName = "u2" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await userService.UnbanUserAsync(user.Id);

            Assert.That(await context.Users.AsNoTracking().CountAsync(u => u.IsActive), Is.EqualTo(1));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}