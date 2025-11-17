using Vortex.Application;
using Vortex.Application.Services;
using Vortex.Infrastructure;
using Vortex.Infrastructure.Interfaces;

namespace Vortex.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructureDependency(configuration);
        services.AddApplicationDependency();
        return services;
    }
}