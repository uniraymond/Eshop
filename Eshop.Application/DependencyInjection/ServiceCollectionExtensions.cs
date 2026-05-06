using Eshop.Application.BackgroundJobs.Interfaces;
using Eshop.Application.BackgroundJobs.Services;
using Eshop.Application.Carts.Interfaces;
using Eshop.Application.Carts.Services;
using Eshop.Application.Common.Interfaces;
using Eshop.Application.Orders.Interfaces;
using Eshop.Application.Orders.Services;
using Eshop.Application.Products.Interfaces;
using Eshop.Application.Products.Services;
using Eshop.Application.Users.Interfaces;
using Eshop.Application.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Eshop.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOutboxProcessor, OutboxProcessor>();

            return services;
        }
    }
}
