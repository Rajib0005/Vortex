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
                configurator.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });
        

        return services;
    }
}