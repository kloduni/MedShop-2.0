using MedShop.Core.Contracts.Admin;
using MedShop.Core.Services.Admin;
using MedShop.Infrastructure.Data;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class AdminUserServiceTests
    {
        private IRepository repo;
        private IUserService userService;
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
        public async Task TestAll_ReturnsCorrectValues()
        {
            var tRepo = new Repository(context);
            userService = new UserService(tRepo);

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
                    IsActive = true,
                    UserName = "user2"
                }
            });
            await tRepo.SaveChangesAsync();

            var model = await userService.All();

            Assert.That(model.First().UserId, Is.EqualTo("3a45d2af-9dfa-4c52-87b8-780a0374b8ab"));
            Assert.That(model.First().UserName, Is.EqualTo("user"));
            Assert.That(model.First().Email, Is.EqualTo("user@medshop.com"));
            Assert.IsTrue(model.First().IsActive);

            Assert.That(model.Reverse().First().UserId, Is.EqualTo("63d65a50-2c24-4943-9d64-66da5aff20b3"));
            Assert.That(model.Reverse().First().UserName, Is.EqualTo("user2"));
            Assert.That(model.Reverse().First().Email, Is.EqualTo("user2@medshop.com"));
            Assert.IsTrue(model.Reverse().First().IsActive);
        }

        [Test]
        public async Task TestBanUser_ChangesActiveStatusCorrectly()
        {
            var tRepo = new Repository(context);
            userService = new UserService(tRepo);

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
                    IsActive = true,
                    UserName = "user2"
                }
            });
            await tRepo.SaveChangesAsync();

            var user = tRepo.All<User>().First();
            var user2 = tRepo.All<User>().Reverse().First();

            await userService.BanUserAsync(user2);

            Assert.That(await tRepo.AllReadonly<User>().CountAsync(u => u.IsActive), Is.EqualTo(1));
        }

        [Test]
        public async Task TestUnbanUser_ChangesActiveStatusCorrectly()
        {
            var tRepo = new Repository(context);
            userService = new UserService(tRepo);

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
            await tRepo.SaveChangesAsync();

            var user = tRepo.All<User>().First();
            var user2 = tRepo.All<User>().Reverse().First();

            await userService.UnbanUserAsync(user2);

            Assert.That(await tRepo.AllReadonly<User>().CountAsync(u => u.IsActive), Is.EqualTo(2));
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
