using Vortex.Application;
using Vortex.Infrastructure;

namespace Vortex.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationDependency();
        services.AddInfrastructureDependency(configuration);
        return services;
    }
}