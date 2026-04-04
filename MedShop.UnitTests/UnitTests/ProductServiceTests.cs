using MedShop.Core.Contracts;
using MedShop.Core.Exceptions;
using MedShop.Core.Models.Product;
using MedShop.Core.Models.Product.ProductSortingEnum;
using MedShop.Core.Services;
using MedShop.Infrastructure.Data;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private ApplicationDbContext dbContext;
        private IApplicationDbContext context;
        private IProductService productService;
        private IGuard guard;

        [SetUp]
        public void Setup()
        {
            guard = new Guard();
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            dbContext = new ApplicationDbContext(contextOptions);
            context = dbContext;

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}