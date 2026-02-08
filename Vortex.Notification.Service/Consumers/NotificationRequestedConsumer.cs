using MassTransit;
using Microsoft.Extensions.Logging;
using Vortex.Contracts;

namespace Vortex.Notification.Service.Consumers;

public class NotificationRequestedConsumer : IConsumer<NotificationRequested>
{
    private readonly ILogger<NotificationRequestedConsumer> _logger;

    public NotificationRequestedConsumer(ILogger<NotificationRequestedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<NotificationRequested> context)
    {
        _logger.LogInformation(
            "Received Notification Request for {Destination}: Type='{Type}', Subject='{Subject}'",
            context.Message.Destination,
            context.Message.Type,
            context.Message.Subject,
            context.Message.Body);

        // In a real implementation, you would add logic here to send
        // an email or push notification based on context.Message.Type
        
        return Task.CompletedTask;
    }
}
