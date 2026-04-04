using MedShop.Core.Cart;
using MedShop.Core.Contracts;
using MedShop.Core.Contracts.Admin;
using MedShop.Core.Exceptions;
using MedShop.Core.Services;
using MedShop.Core.Services.Admin;
using MedShop.Infrastructure.Data;

namespace MedShop.Extensions
{
    public static class MedShopServiceCollection
    {
        public static IServiceCollection AddMedShopServices(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IGuard, Guard>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(sc => ShoppingCart.GetShoppingCart(sc));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWishlistService, WishlistService>();

            return services;
        }
    }
}
