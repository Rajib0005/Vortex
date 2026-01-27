using MassTransit;
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

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                var rabbitMqConfig = configuration.GetSection("RabbitMq");
                var host = rabbitMqConfig["Host"];
                var username = rabbitMqConfig["Username"];
                var password = rabbitMqConfig["Password"];

                configurator.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
            });
        });
        

        return services;
    }
}