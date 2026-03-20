using MedShop.Core.Contracts;
using MedShop.Core.Services;
using MedShop.Infrastructure.Data;
using MedShop.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace MedShop.WebApi.Extensions
{
    public static class MedShopApiServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserStatisticsService, UserStatisticsService>();

            return services;
        }

        public static IServiceCollection AddMedShopDbContext(this IServiceCollection services,
            IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(connectionString));
            services.AddScoped<IRepository, Repository>();

            return services;
        }
    }
}
