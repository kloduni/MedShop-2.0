using MedShop.Core.Cart;
using MedShop.Infrastructure.Data;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Tests.UnitTests
{
    [TestFixture]
    public class ShoppingCartTests
    {
        private IRepository repo;
        private ShoppingCart shoppingCart;
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
        public async Task TestAddItemToCart_AddsItemToCartCorrectly()
        {
            var tRepo = new Repository(context);
            shoppingCart = new ShoppingCart(tRepo);
            shoppingCart.ShoppingCartId = Guid.NewGuid().ToString();

            var product = new Product()
            {
                Id = 1,
                ProductName = "product",
                Description = "",
                ImageUrl = "",
                Price = 10,
                Quantity = 10
            };

            await shoppingCart.AddItemToCartAsync(product);

            Assert.That(await tRepo.AllReadonly<ShoppingCartItem>().CountAsync(), Is.EqualTo(1));

            var shoppingCartitem = await tRepo.AllReadonly<ShoppingCartItem>()
                .Include(i => i.Product)
                .FirstAsync();

            Assert.That(shoppingCartitem.Amount, Is.EqualTo(1));
            Assert.That(product.Id, Is.EqualTo(shoppingCartitem.Product.Id));
            Assert.That(product.ProductName, Is.EqualTo(shoppingCartitem.Product.ProductName));
            Assert.That(product.Price, Is.EqualTo(shoppingCartitem.Product.Price));
            Assert.That(product.Quantity, Is.EqualTo(shoppingCartitem.Product.Quantity));

            await shoppingCart.AddItemToCartAsync(product);

            shoppingCartitem = await tRepo.AllReadonly<ShoppingCartItem>()
                .Include(i => i.Product)
                .FirstAsync();

            Assert.That(shoppingCartitem.Amount, Is.EqualTo(2));
        }

        [Test]
        public async Task TestRemoveItemFromCart_RemovesItemFromCartCorrectly()
        {
            var tRepo = new Repository(context);
            shoppingCart = new ShoppingCart(tRepo);
            shoppingCart.ShoppingCartId = Guid.NewGuid().ToString();

            var product = new Product()
            {
                Id = 1,
                ProductName = "product",
                Description = "",
                ImageUrl = "",
                Price = 10,
                Quantity = 10
            };

            await tRepo.AddAsync(new ShoppingCartItem()
            {
                Id = 1,
                Amount = 2,
                ShoppingCartId = shoppingCart.ShoppingCartId,
                Product = product
            });
            await tRepo.SaveChangesAsync();

            var item = await tRepo.All<ShoppingCartItem>().FirstAsync();

            await shoppingCart.RemoveItemFromCartAsync(item);

            item = await tRepo.All<ShoppingCartItem>().FirstAsync();

            Assert.That(item.Amount, Is.EqualTo(1));

            await shoppingCart.RemoveItemFromCartAsync(item);

            Assert.That(await tRepo.AllReadonly<ShoppingCartItem>().CountAsync(), Is.EqualTo(0));


        }

        [Test]
        public async Task TestGetShoppingCartItems_GetsShoppingItemsCorrectly()
        {
            var tRepo = new Repository(context);
            shoppingCart = new ShoppingCart(tRepo);
            shoppingCart.ShoppingCartId = Guid.NewGuid().ToString();

            var product = new Product()
            {
                Id = 1,
                ProductName = "product",
                Description = "",
                ImageUrl = "",
                Price = 10,
                Quantity = 1
            };
            var product2 = new Product()
            {
                Id = 2,
                ProductName = "product2",
                Description = "",
                ImageUrl = "",
                Price = 20,
                Quantity = 2
            };
            await shoppingCart.AddItemToCartAsync(product);
            await shoppingCart.AddItemToCartAsync(product2);

            var items = shoppingCart.GetShoppingCartItems();

            Assert.That(items.Count, Is.EqualTo(2));

            var item1 = items.First();
            var item2 = items.Last();

            Assert.That(product.ProductName, Is.EqualTo(item1.Product.ProductName));
            Assert.That(product2.ProductName, Is.EqualTo(item2.Product.ProductName));
        }

        [Test]
        public async Task TestGetCartItemById_GetsCorrectItems()
        {
            var tRepo = new Repository(context);
            shoppingCart = new ShoppingCart(tRepo);
            shoppingCart.ShoppingCartId = Guid.NewGuid().ToString();

            var product = new Product()
            {
                Id = 1,
                ProductName = "product",
                Description = "",
                ImageUrl = "",
                Price = 10,
                Quantity = 10
            };
            var product2 = new Product()
            {
                Id = 2,
                ProductName = "product2",
                Description = "",
                ImageUrl = "",
                Price = 20,
                Quantity = 20
            };

            await tRepo.AddAsync(new ShoppingCartItem()
            {
                Id = 1,
                Amount = 2,
                ShoppingCartId = shoppingCart.ShoppingCartId,
                Product = product
            });
            await tRepo.AddAsync(new ShoppingCartItem()
            {
                Id = 2,
                Amount = 2,
                ShoppingCartId = shoppingCart.ShoppingCartId,
                Product = product2
            });
            await tRepo.SaveChangesAsync();

            var item = await shoppingCart.GetCartItemByIdAsync(1);
            var item2 = await shoppingCart.GetCartItemByIdAsync(2);

            Assert.That(item.Id, Is.EqualTo(1));
            Assert.That(item2.Id, Is.EqualTo(2));
        }

        [Test]
        public async Task TestGetShoppingCartTotal_ReturnsCorrectSum()
        {
            var tRepo = new Repository(context);
            shoppingCart = new ShoppingCart(tRepo);
            shoppingCart.ShoppingCartId = Guid.NewGuid().ToString();

            var product = new Product()
            {
                Id = 1,
                ProductName = "product",
                Description = "",
                ImageUrl = "",
                Price = 10,
                Quantity = 1
            };
            var product2 = new Product()
            {
                Id = 2,
                ProductName = "product2",
                Description = "",
                ImageUrl = "",
                Price = 20,
                Quantity = 2
            };

            await tRepo.AddAsync(new ShoppingCartItem()
            {
                Id = 1,
                Amount = 1,
                ShoppingCartId = shoppingCart.ShoppingCartId,
                Product = product
            });
            await tRepo.AddAsync(new ShoppingCartItem()
            {
                Id = 2,
                Amount = 1,
                ShoppingCartId = shoppingCart.ShoppingCartId,
                Product = product2
            });
            await tRepo.SaveChangesAsync();

            var totalSum = await shoppingCart.GetShoppingCartTotalAsync();

            Assert.That(totalSum, Is.EqualTo(30m));
        }

        [Test]
        public async Task TestClearShoppingCart_ClearsCartCorrectly()
        {
            var tRepo = new Repository(context);
            shoppingCart = new ShoppingCart(tRepo);
            shoppingCart.ShoppingCartId = "9cf147b8-412b-455b-b587-68ae68606cf1";

            var product = new Product()
            {
                Id = 1,
                ProductName = "product",
                Description = "",
                ImageUrl = "",
                Price = 10,
                Quantity = 1
            };
            var product2 = new Product()
            {
                Id = 2,
                ProductName = "product2",
                Description = "",
                ImageUrl = "",
                Price = 20,
                Quantity = 2
            };

            await shoppingCart.AddItemToCartAsync(product);
            await shoppingCart.AddItemToCartAsync(product2);

            var items = shoppingCart.GetShoppingCartItems();

            Assert.IsNotNull(items);
            Assert.That((await tRepo.AllReadonly<ShoppingCartItem>().CountAsync()), Is.EqualTo(2));

            await shoppingCart.ClearShoppingCartAsync();


            Assert.That((await tRepo.AllReadonly<ShoppingCartItem>().CountAsync()), Is.EqualTo(0));
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
