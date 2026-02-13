using Vortex.Contracts.Emails;
using MassTransit;
using Vortex.Contracts.Models;
using Vortex.Notification.Service.Interfaces;
using Vortex.Notification.Service.Utils;

namespace Vortex.Notification.Service.Consumers
{
    public class NotificationConsumer(
        ILogger<NotificationConsumer> logger,
        IEmailProvider emailProvider) : IConsumer<NotificationContract>
    {
        public async Task Consume(ConsumeContext<NotificationContract> context)
        {
            var notification = context.Message;
            logger.LogInformation("Processing notification {NotificationId} for {Destination}", notification.NotificationId, notification.Destination);

            if (string.IsNullOrEmpty(notification.Destination))
            {
                logger.LogWarning("Notification {NotificationId} has no Destination, cannot proceed.", notification.NotificationId);
                return;
            }

            try
            {
                var subject = "A message from Vortex";
                if(notification.TemplateData.TryGetValue("Subject", out var subj))
                {
                    subject = subj;
                }

                var body = EmailBodyParser.Parse(notification.TemplateId, notification.TemplateData);
                await emailProvider.SendEmailAsync(new GenericEmail { To = notification.Destination, Subject = subject, Body = body });
                logger.LogInformation("Successfully processed email notification for {Destination}", notification.Destination);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process email notification for {Destination}", notification.Destination);
            }
        }
    }
}
