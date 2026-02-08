using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Vortex.Contracts.Models;
using Vortex.Notification.Service.Interfaces;
using Vortex.Notification.Service.Utils;

namespace Vortex.Notification.Service.Consumers
{
    public class NotificationConsumer : IConsumer<NotificationContract>
    {
        private readonly ILogger<NotificationConsumer> _logger;
        private readonly IEmailProvider _emailProvider;

        public NotificationConsumer(
            ILogger<NotificationConsumer> logger,
            IEmailProvider emailProvider)
        {
            _logger = logger;
            _emailProvider = emailProvider;
        }

        public async Task Consume(ConsumeContext<NotificationContract> context)
        {
            var notification = context.Message;
            _logger.LogInformation("Processing notification {NotificationId} for {Destination}", notification.NotificationId, notification.Destination);

            if (string.IsNullOrEmpty(notification.Destination))
            {
                _logger.LogWarning("Notification {NotificationId} has no Destination, cannot proceed.", notification.NotificationId);
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
                await _emailProvider.SendEmailAsync(notification.Destination, subject, body);
                _logger.LogInformation("Successfully processed email notification for {Destination}", notification.Destination);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to process email notification for {Destination}", notification.Destination);
            }
        }
    }
}
