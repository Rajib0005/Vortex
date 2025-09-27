using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vortex.Infrastructure.Data;

namespace Vortex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependency(this IServiceCollection services,
        IConfiguration configuration)
    {
        Console.WriteLine(configuration.GetConnectionString("DatabaseConnections"));
        services.AddDbContext<VortexDbContext>(options=> 
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnections")));
        return services;
    }
}