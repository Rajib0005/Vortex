using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vortex.Domain.Repositories;
using Vortex.Infrastructure.Data;
using Vortex.Infrastructure.Repositories;

namespace Vortex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependency(this IServiceCollection services,
        IConfiguration configuration)
    {
        Console.WriteLine(configuration.GetConnectionString("DatabaseConnections"));
        // Add DbContext
        services.AddDbContext<VortexDbContext>(options=> 
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnections")));
        // Add Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        return services;
    }
}