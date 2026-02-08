using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Vortex.Infrastructure.Data;
using System.IO;

namespace Vortex.Infrastructure;

public class VortexDbContextFactory : IDesignTimeDbContextFactory<VortexDbContext>
{
    public VortexDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Vortex.API"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        // Create DbContextOptionsBuilder
        var builder = new DbContextOptionsBuilder<VortexDbContext>();
        var connectionString = configuration.GetConnectionString("DatabaseConnections");

        // Use the same Npgsql configuration as in DependencyInjection
        builder.UseNpgsql(connectionString,
            b => b.MigrationsAssembly(typeof(VortexDbContext).Assembly.FullName));

        return new VortexDbContext(builder.Options);
    }
}
