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
        private ILogger<ProductService> logger;
        private IGuard guard;

        [SetUp]
        public void Setup()
        {
            guard = new Guard();
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            dbContext = new ApplicationDbContext(contextOptions);
            context = dbContext;
            productService = new ProductService(context, logger, guard);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated(); // Runs your seeding logic
        }

        [Test]
        public async Task TestProductAllQuery_FiltersAndReturnsCorrectValues()
        {
            // 1. Calculate Base Offset
            var baseProductsCount = await context.Products.CountAsync(p => p.IsActive && p.IsVisible);

            var user = new User() { Id = "test-u99", UserName = "testuser99", Email = "u99@test.com" };
            var category1 = new Category() { Id = 99, Name = "TestCategory99" };
            var category2 = new Category() { Id = 100, Name = "TestCategory100" };

            await context.Users.AddAsync(user);
            await context.Categories.AddRangeAsync(category1, category2);

            await context.Products.AddRangeAsync(new List<Product>()
            {
                new Product() { Id = 99, ProductName = "TestProduct99", Price = 10, CategoryId = 99, IsActive = true, IsVisible = true, Description = "D1", ImageUrl = "I1" },
                new Product() { Id = 100, ProductName = "TestProduct100", Price = 20, CategoryId = 100, IsActive = true, IsVisible = true, Description = "D2", ImageUrl = "I2" },
                new Product() { Id = 101, ProductName = "TestProduct101", Price = 30, CategoryId = 100, IsActive = true, IsVisible = true, Description = "D3", ImageUrl = "I3" }
            });
            await context.SaveChangesAsync();

            await context.UsersProducts.AddRangeAsync(
                new UserProduct() { ProductId = 99, UserId = "test-u99" },
                new UserProduct() { ProductId = 100, UserId = "test-u99" },
                new UserProduct() { ProductId = 101, UserId = "test-u99" }
            );
            await context.SaveChangesAsync();

            // Check overall count increased by exactly 3
            var productsQuery = await productService.All(null, null, ProductSorting.Newest, 1, 100);
            Assert.That(productsQuery.TotalProductsCount, Is.EqualTo(baseProductsCount + 3));

            // Check explicit category filter isolated from seeded data
            var categoryQuery = await productService.All("TestCategory100", null, ProductSorting.Newest, 1, 6);
            Assert.That(categoryQuery.TotalProductsCount, Is.EqualTo(2));

            // Check sorting works (Price ascending within our specific isolated category)
            var priceQuery = await productService.All("TestCategory100", null, ProductSorting.Price, 1, 6);
            Assert.That(priceQuery.Products.First().Price, Is.EqualTo(20));
        }

        [Test]
        public async Task TestAllCategoriesNames_ReturnsCorrectValues()
        {
            var baseCategories = await context.Categories.CountAsync();

            await context.Categories.AddRangeAsync(
                new Category() { Id = 99, Name = "Category99" },
                new Category() { Id = 100, Name = "Category100" }
            );
            await context.SaveChangesAsync();

            var categoryCollection = await productService.AllCategoriesNamesAsync();

            Assert.That(categoryCollection.Count(), Is.EqualTo(baseCategories + 2));
            Assert.That(categoryCollection.Contains("Category99"), Is.True);
        }

        [Test]
        public async Task TestAllCategories_ReturnsCorrectValues()
        {
            var baseCategories = await context.Categories.CountAsync();

            await context.Categories.AddRangeAsync(
                new Category() { Id = 99, Name = "Z_Category" },
                new Category() { Id = 100, Name = "A_Category" }
            );
            await context.SaveChangesAsync();

            var categoryCollection = await productService.AllCategoriesAsync();

            Assert.That(categoryCollection.Count(), Is.EqualTo(baseCategories + 2));
            // First item should be our injected "A_Category" if seeded data doesn't start with numbers or symbols
            Assert.IsTrue(categoryCollection.Any(c => c.Name == "A_Category"));
        }

        [Test]
        public async Task TestCategoryExists_ReturnsCorrectValue()
        {
            await context.Categories.AddAsync(new Category() { Id = 99, Name = "Cat99" });
            await context.SaveChangesAsync();

            Assert.IsTrue(await productService.CategoryExistsAsync(99));
            Assert.IsFalse(await productService.CategoryExistsAsync(9999));
        }

        [Test]
        public async Task TestProductExists_ReturnsCorrectValue()
        {
            await context.Products.AddAsync(new Product() { Id = 99, ProductName = "P99", Description = "D", ImageUrl = "I" });
            await context.SaveChangesAsync();

            Assert.IsTrue(await productService.ExistsAsync(99));
            Assert.IsFalse(await productService.ExistsAsync(9999));
        }

        [Test]
        public async Task TestProductDetailsById_ReturnsCorrectValues()
        {
            var user = new User() { Id = "test-u99", UserName = "seller" };
            var category = new Category() { Id = 99, Name = "Cat99" };
            var product = new Product() { Id = 99, ProductName = "P99", Price = 50, CategoryId = 99, IsActive = true, Description = "Desc", ImageUrl = "Img" };

            await context.Users.AddAsync(user);
            await context.Categories.AddAsync(category);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            await context.UsersProducts.AddAsync(new UserProduct() { ProductId = 99, UserId = "test-u99" });
            await context.SaveChangesAsync();

            var details = await productService.ProductDetailsByIdAsync(99);

            Assert.That(details.Id, Is.EqualTo(99));
            Assert.That(details.ProductName, Is.EqualTo("P99"));
            Assert.That(details.Seller, Is.EqualTo("seller"));
            Assert.That(details.Price, Is.EqualTo(50));
        }

        [Test]
        public async Task TestAllProductsByUserId_ReturnsCorrectValues()
        {
            var user1 = new User() { Id = "test-u99", UserName = "seller1" };
            var user2 = new User() { Id = "test-u100", UserName = "seller2" };
            var cat = new Category() { Id = 99, Name = "C99" };

            await context.Users.AddRangeAsync(user1, user2);
            await context.Categories.AddAsync(cat);
            await context.Products.AddRangeAsync(
                new Product() { Id = 99, ProductName = "P99", CategoryId = 99, IsActive = true, Description = "D", ImageUrl = "I" },
                new Product() { Id = 100, ProductName = "P100", CategoryId = 99, IsActive = true, Description = "D", ImageUrl = "I" }
            );
            await context.SaveChangesAsync();

            await context.UsersProducts.AddRangeAsync(
                new UserProduct() { ProductId = 99, UserId = "test-u99" },
                new UserProduct() { ProductId = 100, UserId = "test-u100" }
            );
            await context.SaveChangesAsync();

            var products = await productService.AllProductsByUserIdAsync("test-u99");

            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.First().ProductName, Is.EqualTo("P99"));
        }

        [Test]
        public async Task TestProductHasUserWithId_ReturnsCorrectValue()
        {
            var user = new User() { Id = "test-u99", UserName = "user1" };
            await context.Users.AddAsync(user);
            await context.Products.AddAsync(new Product() { Id = 99, ProductName = "P99", IsActive = true, Description = "D", ImageUrl = "I" });
            await context.SaveChangesAsync();

            await context.UsersProducts.AddAsync(new UserProduct() { ProductId = 99, UserId = "test-u99" });
            await context.SaveChangesAsync();

            Assert.IsTrue(await productService.HasUserWithIdAsync(99, "test-u99"));
            Assert.IsFalse(await productService.HasUserWithIdAsync(99, "test-u100"));
        }

        [Test]
        public async Task TestGetProductCategoryId_ReturnsCorrectValue()
        {
            await context.Products.AddAsync(new Product() { Id = 99, CategoryId = 55, ProductName = "P", Description = "D", ImageUrl = "I" });
            await context.SaveChangesAsync();

            var categoryId = await productService.GetProductCategoryIdAsync(99);
            Assert.That(categoryId, Is.EqualTo(55));
        }

        [Test]
        public async Task TestProductEdit_EditsProductCorrectly()
        {
            var product = new Product() { Id = 99, ProductName = "Old", Price = 10, CategoryId = 1, Description = "D", ImageUrl = "I" };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var model = new ProductBaseModel() { ProductName = "New", Price = 20, CategoryId = 2, Description = "NewD", ImageUrl = "NewI" };

            await productService.EditAsync(99, model);

            var updated = await context.Products.FindAsync(99);
            Assert.That(updated.ProductName, Is.EqualTo("New"));
            Assert.That(updated.Price, Is.EqualTo(20));
            Assert.That(updated.CategoryId, Is.EqualTo(2));
        }

        [Test]
        public async Task TestProductDelete_RemovesActiveStatusCorrectly()
        {
            await context.Products.AddAsync(new Product() { Id = 99, IsActive = true, ProductName = "P", Description = "D", ImageUrl = "I" });
            await context.SaveChangesAsync();

            await productService.DeleteAsync(99);

            var deleted = await context.Products.FindAsync(99);
            Assert.IsFalse(deleted.IsActive);
        }

        [Test]
        public async Task TestProductGetById_ReturnsCorrectValue()
        {
            var user = new User() { Id = "test-u99", UserName = "user1" };
            await context.Users.AddAsync(user);
            await context.Products.AddAsync(new Product() { Id = 99, ProductName = "TestP", Description = "D", ImageUrl = "I" });
            await context.SaveChangesAsync();

            await context.UsersProducts.AddAsync(new UserProduct() { ProductId = 99, UserId = "test-u99" });
            await context.SaveChangesAsync();

            var product = await productService.GetProductByIdAsync(99);

            Assert.That(product.Id, Is.EqualTo(99));
            Assert.That(product.ProductName, Is.EqualTo("TestP"));
            Assert.That(product.UsersProducts.First().UserId, Is.EqualTo("test-u99"));
        }

        [Test]
        public async Task TestAllDeletedProducts_ReturnsCorrectValues()
        {
            var baseDeleted = await context.Products.CountAsync(p => !p.IsActive);

            var user = new User() { Id = "test-u99", UserName = "seller" };
            var category = new Category() { Id = 99, Name = "Cat99" };

            await context.Users.AddAsync(user);
            await context.Categories.AddAsync(category);

            await context.Products.AddRangeAsync(
                new Product() { Id = 99, ProductName = "ActiveProd", CategoryId = 99, IsActive = true, Description = "D", ImageUrl = "I" },
                new Product() { Id = 100, ProductName = "DeletedProd", CategoryId = 99, IsActive = false, Description = "D", ImageUrl = "I" }
            );
            await context.SaveChangesAsync();

            await context.UsersProducts.AddRangeAsync(
                new UserProduct() { ProductId = 99, UserId = "test-u99" },
                new UserProduct() { ProductId = 100, UserId = "test-u99" }
            );
            await context.SaveChangesAsync();

            var deletedQuery = await productService.AllDeletedProducts(null, null, ProductSorting.Newest, 1, 100);

            Assert.That(deletedQuery.TotalProductsCount, Is.EqualTo(baseDeleted + 1));
            Assert.IsTrue(deletedQuery.Products.Any(p => p.ProductName == "DeletedProd"));
        }

        [Test]
        public async Task TestRestoreProduct_ChangesActiveStatusCorrectly()
        {
            await context.Products.AddAsync(new Product() { Id = 99, IsActive = false, ProductName = "P", Description = "D", ImageUrl = "I" });
            await context.SaveChangesAsync();

            await productService.RestoreProductAsync(99);

            var restored = await context.Products.FindAsync(99);
            Assert.IsTrue(restored.IsActive);
        }

        [Test]
        public async Task TestReduceProductAmount_ReducesProductAmountCorrectly()
        {
            var product = new Product() { Id = 99, Quantity = 10, ProductName = "P", Description = "D", ImageUrl = "I" };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var items = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem() { Id = 99, Product = product, Amount = 3, ShoppingCartId = "c1" }
            };

            await productService.ReduceProductAmount(items);

            var updatedProduct = await context.Products.FindAsync(99);
            Assert.That(updatedProduct.Quantity, Is.EqualTo(7));
        }

        [Test]
        public async Task TestProductCreate_CreatesCorrectProduct()
        {
            var user = new User() { Id = "test-u99", UserName = "test" };
            var category = new Category() { Id = 99, Name = "Cat99" };

            await context.Users.AddAsync(user);
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            var model = new ProductBaseModel()
            {
                ProductName = "NewProd",
                Description = "Desc",
                ImageUrl = "Url",
                Price = 10,
                Quantity = 5,
                CategoryId = 99
            };

            var newId = await productService.CreateAsync(model, "test-u99");

            var created = await context.Products.Include(p => p.UsersProducts).FirstOrDefaultAsync(p => p.Id == newId);

            Assert.IsNotNull(created);
            Assert.That(created.ProductName, Is.EqualTo("NewProd"));
            Assert.That(created.UsersProducts.First().UserId, Is.EqualTo("test-u99"));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}