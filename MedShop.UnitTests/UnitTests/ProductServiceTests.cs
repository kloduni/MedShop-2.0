using MedShop.Core.Contracts;
using MedShop.Core.Exceptions;
using MedShop.Core.Models.Product;
using MedShop.Core.Models.Product.ProductSortingEnum;
using MedShop.Core.Services;
using MedShop.Infrastructure.Data;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private IRepository repo;
        private IProductService productService;
        private ILogger<ProductService> logger;
        private IGuard guard;
        private ApplicationDbContext context;

        [SetUp]
        public void Setup()
        {
            guard = new Guard();
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("MedShopTestDb")
                .Options;

            context = new ApplicationDbContext(contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Test]
        public async Task TestCarouselProductsCount()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddRangeAsync(new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    ProductName = "",
                    ImageUrl = "",
                    Description = ""
                },
                new Product()
                {
                    Id = 2,
                    ProductName = "",
                    ImageUrl = "",
                    Description = ""
                },
                new Product()
                {
                    Id = 3,
                    ProductName = "",
                    ImageUrl = "",
                    Description = ""
                },
                new Product()
                {
                    Id = 4,
                    ProductName = "",
                    ImageUrl = "",
                    Description = ""
                }
            });
            await tRepo.SaveChangesAsync();

            var productCollection = await productService.AllCarousel();

            //test product count
            Assert.That(productCollection.Count(), Is.EqualTo(4));
        }

        [Test]
        public async Task TestProductAllQuery_FiltersAndReturnsCorrectValues()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddRangeAsync(new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    ProductName = "TestProduct1",
                    Description = "TestDescription1",
                    ImageUrl = "TestUrl1",
                    Price = 10,
                    Category = new Category()
                    {
                        Id = 1,
                        Name = "TestCategory1",
                    },
                    Quantity = 10,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 1,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser1@medshop.com",
                                UserName = "testuser1",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 2,
                    ProductName = "TestProduct2",
                    Description = "TestDescription2",
                    ImageUrl = "TestUrl2",
                    Price = 20,
                    Category = new Category()
                    {
                        Id = 2,
                        Name = "TestCategory2",
                    },
                    Quantity = 20,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 2,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser2@medshop.com",
                                UserName = "testuser2",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 3,
                    ProductName = "TestProduct3",
                    Description = "TestDescription3",
                    ImageUrl = "TestUrl3",
                    Price = 30,
                    Category = new Category()
                    {
                        Id = 3,
                        Name = "TestCategory3",
                    },
                    Quantity = 30,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 3,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser3@medshop.com",
                                UserName = "testuser3",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 4,
                    ProductName = "TestProduct4",
                    Description = "TestDescription4",
                    ImageUrl = "TestUrl4",
                    Price = 40,
                    Category = new Category()
                    {
                        Id = 4,
                        Name = "TestCategory4",
                    },
                    Quantity = 40,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 4,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser4@medshop.com",
                                UserName = "testuser4",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                }
            });

            await tRepo.SaveChangesAsync();

            var productsQuery = await productService.All(null, null, ProductSorting.Newest, 1, 6);

            //test count and newest order
            Assert.That(productsQuery.TotalProductsCount, Is.EqualTo(4));
            Assert.That(productsQuery.Products.First().Id, Is.EqualTo(4));

            //test products per page
            var productsQuery2 = await productService.All(null, null, ProductSorting.Newest, 1, 2);
            Assert.That(productsQuery2.Products.Count(), Is.EqualTo(2));

            //test price order
            var productsQuery3 = await productService.All(null, null, ProductSorting.Price, 1, 6);
            Assert.That(productsQuery3.Products.First().Price, Is.EqualTo(10));

            //test category order
            var categoryQuery = await productService.All("TestCategory2", null, ProductSorting.Newest, 1, 6);
            Assert.That(categoryQuery.Products.First().ProductName, Is.EqualTo("TestProduct2"));

            //test searchTerm order
            var searchTermQuery = await productService.All(null, "TestProduct1", ProductSorting.Newest, 1, 6);
            Assert.That(searchTermQuery.Products.First().Id, Is.EqualTo(1));
        }

        [Test]
        public async Task TestAllCategoriesNames_ReturnsCorrectValues()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddRangeAsync(new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    Name = "Category1"
                },
                new Category()
                {
                    Id = 2,
                    Name = "Category2"
                },
                new Category()
                {
                    Id = 3,
                    Name = "Category3"
                },
                new Category()
                {
                    Id = 4,
                    Name = "Category4"
                },
            });
            await tRepo.SaveChangesAsync();

            var categoryCollection = await productService.AllCategoriesNamesAsync();

            Assert.That(categoryCollection.Count(), Is.EqualTo(4));
            Assert.That(categoryCollection.First(), Is.EqualTo("Category1"));
        }

        [Test]
        public async Task TestAllCategories_ReturnsCorrectValues()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddRangeAsync(new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    Name = "Category1"
                },
                new Category()
                {
                    Id = 2,
                    Name = "Category2"
                },
                new Category()
                {
                    Id = 3,
                    Name = "Category3"
                },
                new Category()
                {
                    Id = 4,
                    Name = "Category4"
                },
            });
            await tRepo.SaveChangesAsync();

            var categoryCollection = await productService.AllCategoriesAsync();

            Assert.That(categoryCollection.Count(), Is.EqualTo(4));
            Assert.That(categoryCollection.First().Name, Is.EqualTo("Category1"));
        }

        [Test]
        public async Task TestCategoryExists_ReturnsCorrectValue()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            var result = await productService.CategoryExistsAsync(1);

            Assert.That(result, Is.False);

            await tRepo.AddRangeAsync(new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    Name = "Category1"
                },
                new Category()
                {
                    Id = 2,
                    Name = "Category2"
                },
                new Category()
                {
                    Id = 3,
                    Name = "Category3"
                },
                new Category()
                {
                    Id = 4,
                    Name = "Category4"
                },
            });
            await tRepo.SaveChangesAsync();

            var result2 = await productService.CategoryExistsAsync(1);

            Assert.IsTrue(result2);

            var result3 = await productService.CategoryExistsAsync(3);

            Assert.IsTrue(result3);
        }

        [Test]
        public async Task TestProductExists_ReturnsCorrectValue()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            var result = await productService.ExistsAsync(1);

            Assert.IsFalse(result);

            await tRepo.AddRangeAsync(new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    ProductName = "",
                    ImageUrl = "",
                    Description = ""
                },
                new Product()
                {
                    Id = 2,
                    ProductName = "",
                    ImageUrl = "",
                    Description = ""
                },
                new Product()
                {
                    Id = 3,
                    ProductName = "",
                    ImageUrl = "",
                    Description = ""
                },
                new Product()
                {
                    Id = 4,
                    ProductName = "",
                    ImageUrl = "",
                    Description = ""
                }
            });
            await tRepo.SaveChangesAsync();

            var result2 = await productService.ExistsAsync(1);

            Assert.IsTrue(result2);

            var result3 = await productService.ExistsAsync(3);

            Assert.IsTrue(result3);
        }

        [Test]
        public async Task TestProductDetailsById_ReturnsCorrectValues()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddRangeAsync(new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    ProductName = "TestProduct1",
                    Description = "TestDescription1",
                    ImageUrl = "TestUrl1",
                    Price = 10,
                    Category = new Category()
                    {
                        Id = 1,
                        Name = "TestCategory1",
                    },
                    Quantity = 10,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 1,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser1@medshop.com",
                                UserName = "testuser1",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 2,
                    ProductName = "TestProduct2",
                    Description = "TestDescription2",
                    ImageUrl = "TestUrl2",
                    Price = 20,
                    Category = new Category()
                    {
                        Id = 2,
                        Name = "TestCategory2",
                    },
                    Quantity = 20,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 2,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser2@medshop.com",
                                UserName = "testuser2",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 3,
                    ProductName = "TestProduct3",
                    Description = "TestDescription3",
                    ImageUrl = "TestUrl3",
                    Price = 30,
                    Category = new Category()
                    {
                        Id = 3,
                        Name = "TestCategory3",
                    },
                    Quantity = 30,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 3,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser3@medshop.com",
                                UserName = "testuser3",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 4,
                    ProductName = "TestProduct4",
                    Description = "TestDescription4",
                    ImageUrl = "TestUrl4",
                    Price = 40,
                    Category = new Category()
                    {
                        Id = 4,
                        Name = "TestCategory4",
                    },
                    Quantity = 40,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 4,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser4@medshop.com",
                                UserName = "testuser4",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                }
            });

            await tRepo.SaveChangesAsync();

            var product = await productService.ProductDetailsByIdAsync(4);

            Assert.That(product.Id, Is.EqualTo(4));
            Assert.That(product.ProductName, Is.EqualTo("TestProduct4"));

            var product2 = await productService.ProductDetailsByIdAsync(2);

            Assert.That(product2.Description, Is.EqualTo("TestDescription2"));
            Assert.That(product2.ImageUrl, Is.EqualTo("TestUrl2"));
            Assert.That(product2.Price, Is.EqualTo(20));
            Assert.That(product2.Category, Is.EqualTo("TestCategory2"));

            var product3 = await productService.ProductDetailsByIdAsync(3);

            Assert.That(product3.Quantity, Is.EqualTo(30));
            Assert.That(product3.Seller, Is.EqualTo("testuser3"));
        }

        [Test]
        public async Task TestAllProductsByUserId_ReturnsCorrectValues()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddRangeAsync(new List<Product>()
            {
                new Product()
                {
                    Id = 2,
                    ProductName = "TestProduct2",
                    Description = "TestDescription2",
                    ImageUrl = "TestUrl2",
                    Price = 20,
                    Category = new Category()
                    {
                        Id = 2,
                        Name = "TestCategory2",
                    },
                    Quantity = 20,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 2,
                            User = new User()
                            {
                                Id = "9cf147b8-412b-455b-b587-68ae68606cf1",
                                Email = "testuser2@medshop.com",
                                UserName = "testuser2",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 3,
                    ProductName = "TestProduct3",
                    Description = "TestDescription3",
                    ImageUrl = "TestUrl3",
                    Price = 30,
                    Category = new Category()
                    {
                        Id = 3,
                        Name = "TestCategory3",
                    },
                    Quantity = 30,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 3,
                            User = new User()
                            {
                                Id = "3a45d2af-9dfa-4c52-87b8-780a0374b8ab",
                                Email = "testuser3@medshop.com",
                                UserName = "testuser3",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 4,
                    ProductName = "TestProduct4",
                    Description = "TestDescription4",
                    ImageUrl = "TestUrl4",
                    Price = 40,
                    Category = new Category()
                    {
                        Id = 4,
                        Name = "TestCategory4",
                    },
                    Quantity = 40,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 4,
                            User = new User()
                            {
                                Id = "63d65a50-2c24-4943-9d64-66da5aff20b3",
                                Email = "testuser4@medshop.com",
                                UserName = "testuser4",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                }
            });

            await tRepo.SaveChangesAsync();

            var products = await productService.AllProductsByUserIdAsync("63d65a50-2c24-4943-9d64-66da5aff20b3");

            Assert.That(products.First().Id, Is.EqualTo(4));
            Assert.That(products.First().ProductName, Is.EqualTo("TestProduct4"));

            await tRepo.AddAsync(new User()
            {
                Id = "4d6a0eea-e004-4461-8383-05ee6dd486f0",
                UserName = "testuser1",
                Email = "testuser1@medshop.com",
                EmailConfirmed = true,
                IsActive = true,
                UsersProducts = new List<UserProduct>()
                {
                    new UserProduct()
                    {
                        UserId = "4d6a0eea-e004-4461-8383-05ee6dd486f0",
                        Product = new Product()
                        {
                            Id = 6,
                            ProductName = "TestProduct6",
                            Description = "TestDescription6",
                            ImageUrl = "TestUrl6",
                            Price = 60,
                            Category = new Category()
                            {
                                Id = 6,
                                Name = "TestCategory6",
                            },
                            Quantity = 60,
                            IsActive = true,
                            }
                    },
                    new UserProduct()
                    {
                        UserId = "4d6a0eea-e004-4461-8383-05ee6dd486f0",
                        Product = new Product()
                        {
                            Id = 7,
                            ProductName = "TestProduct7",
                            Description = "TestDescription7",
                            ImageUrl = "TestUrl7",
                            Price = 70,
                            Category = new Category()
                            {
                                Id = 7,
                                Name = "TestCategory7",
                            },
                            Quantity = 70,
                            IsActive = true,
                        }
                    }
                }
            });
            await tRepo.SaveChangesAsync();


            var products2 = await productService.AllProductsByUserIdAsync("4d6a0eea-e004-4461-8383-05ee6dd486f0");

            Assert.That(products2.Count(), Is.EqualTo(2));
            Assert.That(products2.First().ProductName, Is.EqualTo("TestProduct7"));
            Assert.That(products2.Reverse().First().ProductName, Is.EqualTo("TestProduct6"));
        }

        [Test]
        public async Task TestProductHasUserWithId_ReturnsCorrectValue()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddAsync(new Product()
            {
                Id = 1,
                ProductName = "TestProduct1",
                Description = "TestDescription1",
                ImageUrl = "TestUrl1",
                Price = 10,
                Category = new Category()
                {
                    Id = 1,
                    Name = "TestCategory1",
                },
                Quantity = 10,
                IsActive = true,
                UsersProducts = new List<UserProduct>()
                {
                    new UserProduct()
                    {
                        ProductId = 1,
                        User = new User()
                        {
                            Id = "4d6a0eea-e004-4461-8383-05ee6dd486f0",
                            Email = "testuser1@medshop.com",
                            UserName = "testuser1",
                            EmailConfirmed = true,
                            IsActive = true
                        }
                    }
                }
            });
            await tRepo.SaveChangesAsync();

            var result = await productService.HasUserWithIdAsync(1, "4d6a0eea-e004-4461-8383-05ee6dd486f0");

            Assert.IsTrue(result);

            var result2 = await productService.HasUserWithIdAsync(2, "4d6a0eea-e004-4461-8383-05ee6dd486f0");

            Assert.IsFalse(result2);
        }

        [Test]
        public async Task TestGetProductCategoryId_ReturnsCorrectValue()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddAsync(new Product()
            {
                Id = 1,
                ProductName = "TestProduct1",
                Description = "TestDescription1",
                ImageUrl = "TestUrl1",
                Price = 10,
                Category = new Category()
                {
                    Id = 2,
                    Name = "TestCategory2",
                },
            });
            await tRepo.AddAsync(new Product()
            {
                Id = 2,
                ProductName = "TestProduct2",
                Description = "TestDescription2",
                ImageUrl = "TestUrl2",
                Price = 20,
                Category = new Category()
                {
                    Id = 1,
                    Name = "TestCategory1",
                },
            });
            await tRepo.SaveChangesAsync();

            var categoryId = await productService.GetProductCategoryIdAsync(1);

            Assert.That(categoryId, Is.EqualTo(2));

            var categoryId2 = await productService.GetProductCategoryIdAsync(2);

            Assert.That(categoryId2, Is.EqualTo(1));
        }

        [Test]
        public async Task TestProductEdit_EditsProductCorrectly()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddAsync(new Product()
            {
                Id = 4,
                ProductName = "TestProduct4",
                Description = "TestDescription4",
                ImageUrl = "TestUrl4",
                Price = 40,
                Category = new Category()
                {
                    Id = 4,
                    Name = "TestCategory4",
                },
                Quantity = 40,
                IsActive = true,
                UsersProducts = new List<UserProduct>()
                {
                    new UserProduct()
                    {
                        ProductId = 4,
                        User = new User()
                        {
                            Id = "63d65a50-2c24-4943-9d64-66da5aff20b3",
                            Email = "testuser4@medshop.com",
                            UserName = "testuser4",
                            EmailConfirmed = true,
                            IsActive = true
                        }
                    }
                }
            });
            await tRepo.AddAsync(new Category()
            {
                Id = 1,
                Name = "NewCategory",
            });
            await tRepo.SaveChangesAsync();


            var productModel = new ProductBaseModel()
            {
                ProductName = "NewName",
                Description = "NewDescription",
                Price = 123,
                ImageUrl = "NewUrl",
                CategoryId = 1,
                Quantity = 123
            };

            await productService.EditAsync(4, productModel);

            var product = await tRepo.AllReadonly<Product>().FirstAsync(p => p.Id == 4);

            Assert.That(product.ProductName, Is.EqualTo("NewName"));
            Assert.That(product.Description, Is.EqualTo("NewDescription"));
            Assert.That(product.Price, Is.EqualTo(123));
            Assert.That(product.ImageUrl, Is.EqualTo("NewUrl"));
            Assert.That(product.CategoryId, Is.EqualTo(1));
            Assert.That(product.Quantity, Is.EqualTo(123));
        }

        [Test]
        public async Task TestProductDelete_RemovesActiveStatusCorrectly()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddAsync(new Product()
            {
                Id = 4,
                ProductName = "TestProduct4",
                Description = "TestDescription4",
                ImageUrl = "TestUrl4",
                Price = 40,
                Category = new Category()
                {
                    Id = 4,
                    Name = "TestCategory4",
                },
                Quantity = 40,
                IsActive = true,
                UsersProducts = new List<UserProduct>()
                {
                    new UserProduct()
                    {
                        ProductId = 4,
                        User = new User()
                        {
                            Id = "63d65a50-2c24-4943-9d64-66da5aff20b3",
                            Email = "testuser4@medshop.com",
                            UserName = "testuser4",
                            EmailConfirmed = true,
                            IsActive = true
                        }
                    }
                }
            });
            await tRepo.SaveChangesAsync();

            await productService.DeleteAsync(4);

            await tRepo.SaveChangesAsync();

            var product = await tRepo.AllReadonly<Product>().Where(p => p.Id == 4).FirstAsync();

            Assert.IsFalse(product.IsActive);
        }

        [Test]
        public async Task TestProductGetById_ReturnsCorrectValue()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddAsync(new Product()
            {
                Id = 4,
                ProductName = "TestProduct4",
                Description = "TestDescription4",
                ImageUrl = "TestUrl4",
                Price = 40,
                Category = new Category()
                {
                    Id = 4,
                    Name = "TestCategory4",
                },
                Quantity = 40,
                IsActive = true,
                UsersProducts = new List<UserProduct>()
                {
                    new UserProduct()
                    {
                        ProductId = 4,
                        User = new User()
                        {
                            Id = "63d65a50-2c24-4943-9d64-66da5aff20b3",
                            Email = "testuser4@medshop.com",
                            UserName = "testuser4",
                            EmailConfirmed = true,
                            IsActive = true
                        }
                    }
                }
            });
            await tRepo.SaveChangesAsync();

            var product = await productService.GetProductByIdAsync(4);

            Assert.That(product.Id, Is.EqualTo(4));
            Assert.That(product.ProductName, Is.EqualTo("TestProduct4"));
            Assert.That(product.UsersProducts.Select(up => up.User.UserName).First(), Is.EqualTo("testuser4"));
        }

        [Test]
        public async Task TestAllDeletedProducts_ReturnsCorrectValues()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            await tRepo.AddRangeAsync(new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    ProductName = "TestProduct1",
                    Description = "TestDescription1",
                    ImageUrl = "TestUrl1",
                    Price = 10,
                    Category = new Category()
                    {
                        Id = 1,
                        Name = "TestCategory1",
                    },
                    Quantity = 10,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 1,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser1@medshop.com",
                                UserName = "testuser1",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 2,
                    ProductName = "TestProduct2",
                    Description = "TestDescription2",
                    ImageUrl = "TestUrl2",
                    Price = 20,
                    Category = new Category()
                    {
                        Id = 2,
                        Name = "TestCategory2",
                    },
                    Quantity = 20,
                    IsActive = false,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 2,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser2@medshop.com",
                                UserName = "testuser2",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 3,
                    ProductName = "TestProduct3",
                    Description = "TestDescription3",
                    ImageUrl = "TestUrl3",
                    Price = 30,
                    Category = new Category()
                    {
                        Id = 3,
                        Name = "TestCategory3",
                    },
                    Quantity = 30,
                    IsActive = true,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 3,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser3@medshop.com",
                                UserName = "testuser3",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                },
                new Product()
                {
                    Id = 4,
                    ProductName = "TestProduct4",
                    Description = "TestDescription4",
                    ImageUrl = "TestUrl4",
                    Price = 40,
                    Category = new Category()
                    {
                        Id = 4,
                        Name = "TestCategory4",
                    },
                    Quantity = 40,
                    IsActive = false,
                    UsersProducts = new List<UserProduct>()
                    {
                        new UserProduct()
                        {
                            ProductId = 4,
                            User = new User()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = "testuser4@medshop.com",
                                UserName = "testuser4",
                                EmailConfirmed = true,
                                IsActive = true
                            }
                        }
                    }
                }
            });

            await tRepo.SaveChangesAsync();

            var productsQuery = await productService.AllDeletedProducts(null, null, ProductSorting.Newest, 1, 6);

            Assert.That(productsQuery.TotalProductsCount, Is.EqualTo(2));

            Assert.That(productsQuery.Products.First().Id, Is.EqualTo(4));

            //test products per page
            var productsQuery2 = await productService.AllDeletedProducts(null, null, ProductSorting.Newest, 1, 1);
            Assert.That(productsQuery2.Products.Count(), Is.EqualTo(1));

            //test price order
            var productsQuery3 = await productService.AllDeletedProducts(null, null, ProductSorting.Price, 1, 6);
            Assert.That(productsQuery3.Products.First().Price, Is.EqualTo(20));

            //test category order
            var categoryQuery = await productService.AllDeletedProducts("TestCategory2", null, ProductSorting.Newest, 1, 6);
            Assert.That(categoryQuery.Products.First().ProductName, Is.EqualTo("TestProduct2"));

            //test searchTerm order
            var searchTermQuery = await productService.AllDeletedProducts(null, "TestProduct2", ProductSorting.Newest, 1, 6);
            Assert.That(searchTermQuery.Products.First().Id, Is.EqualTo(2));

            var searchTermQuery2 = await productService.AllDeletedProducts(null, "TestProduct1", ProductSorting.Newest, 1, 6);
            Assert.That(searchTermQuery2.Products.Count(), Is.EqualTo(0));

        }

        [Test]
        public async Task TestRestoreProduct_ChangesActiveStatusCorrectly()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            var product = new Product()
            {
                Id = 1,
                ProductName = "test",
                Description = "test",
                ImageUrl = "test",
                IsActive = false
            };

            await tRepo.AddAsync(product);
            await tRepo.SaveChangesAsync();

            var deletedProduct = await tRepo.AllReadonly<Product>().FirstAsync();

            Assert.IsFalse(deletedProduct.IsActive);

            await productService.RestoreProductAsync(deletedProduct.Id);

            deletedProduct = await tRepo.AllReadonly<Product>().FirstAsync();

            Assert.IsTrue(deletedProduct.IsActive);
        }

        [Test]
        public async Task TestReduceProductAmount_ReducesProductAmountCorrectly()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            var productList = new List<Product>();

            var product = new Product()
            {
                Id = 1,
                ProductName = "TestProduct1",
                Description = "TestDescription1",
                ImageUrl = "TestUrl1",
                Price = 10,
                Category = new Category()
                {
                    Id = 1,
                    Name = "TestCategory1",
                },
                Quantity = 10,
                IsActive = true,
                UsersProducts = new List<UserProduct>()
                {
                    new UserProduct()
                    {
                        ProductId = 1,
                        User = new User()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = "testuser1@medshop.com",
                            UserName = "testuser1",
                            EmailConfirmed = true,
                            IsActive = true
                        }
                    }
                }
            };

            var product2 = new Product()
            {
                Id = 2,
                ProductName = "TestProduct2",
                Description = "TestDescription2",
                ImageUrl = "TestUrl2",
                Price = 20,
                Category = new Category()
                {
                    Id = 2,
                    Name = "TestCategory2",
                },
                Quantity = 20,
                IsActive = false,
                UsersProducts = new List<UserProduct>()
                {
                    new UserProduct()
                    {
                        ProductId = 2,
                        User = new User()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = "testuser2@medshop.com",
                            UserName = "testuser2",
                            EmailConfirmed = true,
                            IsActive = true
                        }
                    }
                }
            };

            productList.Add(product);
            productList.Add(product2);

            await tRepo.AddRangeAsync(productList);

            var itemList = new List<ShoppingCartItem>();

            var item = new ShoppingCartItem()
            {
                Id = 1,
                ShoppingCartId = "660a95a0-30e3-4add-99d2-adf04555aaff",
                Amount = 5,
                Product = product
            };
            var item2 = new ShoppingCartItem()
            {
                Id = 2,
                ShoppingCartId = "660a95a0-30e3-4add-99d2-adf04555aaff",
                Amount = 10,
                Product = product2
            };

            itemList.Add(item);
            itemList.Add(item2);

            await tRepo.SaveChangesAsync();

            await productService.ReduceProductAmount(itemList);

            var reducedProduct = await tRepo.AllReadonly<Product>().FirstAsync(p => p.Id == 1);
            var reducedProduct2 = await tRepo.AllReadonly<Product>().FirstAsync(p => p.Id == 2);

            Assert.That(reducedProduct.Quantity, Is.EqualTo(5));
            Assert.That(reducedProduct2.Quantity, Is.EqualTo(10));
        }

        [Test]
        public async Task TestProductCreate_CreatesCorrectProduct()
        {
            var loggerMock = new Mock<ILogger<ProductService>>();
            logger = loggerMock.Object;
            var tRepo = new Repository(context);
            productService = new ProductService(tRepo, logger, guard);

            var user = new User()
            {
                Id = "3a45d2af-9dfa-4c52-87b8-780a0374b8ab",
                Email = "user@medshop.com",
                EmailConfirmed = true,
                IsActive = true,
                UserName = "user"
            };

            await tRepo.AddAsync(user);

            var category = new Category()
            {
                Id = 1,
                Name = "category"
            };

            await tRepo.AddAsync(category);
            await tRepo.SaveChangesAsync();

            var model = new ProductBaseModel()
            {
                ProductName = "product",
                Description = "",
                ImageUrl = "",
                Price = 10,
                Quantity = 10,
                CategoryId = 1
            };

            await productService.CreateAsync(model, user.Id);

            var product = await tRepo.AllReadonly<Product>()
                .Include(p => p.UsersProducts)
                .FirstOrDefaultAsync();

            Assert.IsNotNull(product);
            Assert.That(product.ProductName, Is.EqualTo("product"));
            Assert.That(product.Description, Is.EqualTo(""));
            Assert.That(product.ImageUrl, Is.EqualTo(""));
            Assert.That(product.Price, Is.EqualTo(10));
            Assert.That(product.Quantity, Is.EqualTo(10));
            Assert.That(product.CategoryId, Is.EqualTo(1));
            Assert.That(product.UsersProducts.Count, Is.EqualTo(1));
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
