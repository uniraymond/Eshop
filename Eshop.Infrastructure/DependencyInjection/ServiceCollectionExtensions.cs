using Eshop.Application.Auth.Interfaces;
using Eshop.Application.Carts.Interfaces;
using Eshop.Application.Common.Interfaces;
using Eshop.Application.Products.Interfaces;
using Eshop.Domain.Repositories;
using Eshop.Infrastructure.Authentication;
using Eshop.Infrastructure.Caching;
using Eshop.Infrastructure.Data;
using Eshop.Infrastructure.Options;
using Eshop.Infrastructure.Persistence.Repositories;
using Eshop.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Eshop.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
                return ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
            });

            services.AddScoped<ICacheService, RedisCacheService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }
    }
}
