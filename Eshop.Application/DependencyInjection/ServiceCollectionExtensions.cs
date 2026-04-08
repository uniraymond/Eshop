

using Eshop.Application.Common.Interfaces;
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
            return services;
        }
    }
}
