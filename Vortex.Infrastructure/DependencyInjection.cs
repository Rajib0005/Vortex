using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vortex.Domain.Repositories;
using Vortex.Infrastructure.Data;
using Vortex.Infrastructure.Repositories;
using Vortex.Application.Interfaces;
using Vortex.Infrastructure.Interceptors;
using Vortex.Infrastructure.Services;

namespace Vortex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependency(this IServiceCollection services,
        IConfiguration configuration)
    {
        Console.WriteLine(configuration.GetConnectionString("DatabaseConnections"));
        // Register Audit Services
        services.AddScoped<ICorrelationIdService, CorrelationIdService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AuditableEntityInterceptor>();

        // Add DbContext
        services.AddDbContext<VortexDbContext>((sp, options) => 
        {
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnections"));
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });
        // Add Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        return services;
    }
}