using Microsoft.Extensions.DependencyInjection;
using Vortex.Application.Services;
using Vortex.Infrastructure.Interfaces;

namespace Vortex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependency(this IServiceCollection services)
    {
        // Auth
        services.AddScoped<ITokenService, TokenService>();
        
        return services;
    }
}