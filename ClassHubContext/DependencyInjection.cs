using Microsoft.Extensions.DependencyInjection;
using ClassHub.ClassHubContext.Services;

namespace ClassHub.ClassHubContext
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBackendServices(this IServiceCollection services)
        {
            services.AddScoped<UserService>();
            return services;
        }
    }
}
