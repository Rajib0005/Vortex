using MassTransit;
using Vortex.Notification.Service.Consumers;
using Vortex.Notification.Service.Interfaces;
using Vortex.Notification.Service.Models;
using Vortex.Notification.Service.Providers;

var builder = Host.CreateApplicationBuilder(args);

// Register services
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

#region Add Smtp Config
builder.Services.AddSingleton<IEmailProvider, SmtpEmailProvider>();
#endregion

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
            // Retry Policy
            e.UseMessageRetry(r => 
                r.Incremental(
                    3, 
                    TimeSpan.FromSeconds(2), 
                    TimeSpan.FromSeconds(5))
                );
            e.ConfigureConsumer<NotificationConsumer>(context);
        });
    });
});

#endregion

var host = builder.Build();
host.Run();