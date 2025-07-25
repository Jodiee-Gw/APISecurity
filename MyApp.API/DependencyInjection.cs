using MyApp.Application;
using MyApp.Infrastructure;

namespace MyApp.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {
            services.AddApplicationServices().AddInfrastructureServices();
            return services;
        }
    }
}
