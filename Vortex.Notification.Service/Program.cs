using MassTransit;
using Vortex.Notification.Service.Consumers;
using Vortex.Notification.Service.Interfaces;
using Vortex.Notification.Service.Providers;

var builder = Host.CreateApplicationBuilder(args);

// Register services
builder.Services.AddScoped<IEmailProvider, LogEmailProvider>();

#region MassTransit and Rabbitmq
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<NotificationConsumer>();

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

        configurator.ReceiveEndpoint("notifications", e =>
        {
            e.ConfigureConsumer<NotificationConsumer>(context);
        });
    });
});

#endregion

var host = builder.Build();
host.Run();