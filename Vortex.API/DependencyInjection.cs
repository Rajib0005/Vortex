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

        var rabbitMqSettings = configuration.GetSection("RabbitMq");

        var host = rabbitMqSettings["Host"] ?? throw new InvalidOperationException("RabbitMQ host not found");
        var username = rabbitMqSettings["Username"] ?? throw new InvalidOperationException("RabbitMQ username not found");
        var password = rabbitMqSettings["Password"] ?? throw new InvalidOperationException("RabbitMQ password not found");

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host,"/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
            });
        });
        

        return services;
    }
}