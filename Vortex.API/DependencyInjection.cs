using Vortex.Infrastructure;

namespace Vortex.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructureDependency(configuration);
        return services;
    }
}