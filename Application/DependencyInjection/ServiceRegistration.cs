
using Application.Interfaces;
using Application.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace E_Shopping.Application.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICartService, CartManager>();
            services.AddScoped<IAdvertisementService, AdvertisementManager>();
        }
    }
}
