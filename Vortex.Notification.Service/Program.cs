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
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMq");
        var host = rabbitMqConfig["Host"];
        var username = rabbitMqConfig["Username"];
        var password = rabbitMqConfig["Password"];

        configurator.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        configurator.ReceiveEndpoint("notification-requests", e =>
        {
            e.ConfigureConsumer<NotificationRequestedConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();