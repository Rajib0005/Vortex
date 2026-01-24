using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vortex.Notification.Service.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<NotificationRequestedConsumer>();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("admin");
        });

        configurator.ReceiveEndpoint("notification-requests", e =>
        {
            e.ConfigureConsumer<NotificationRequestedConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();