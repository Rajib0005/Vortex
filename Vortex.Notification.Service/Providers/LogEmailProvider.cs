using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vortex.Contracts.Emails;
using Vortex.Notification.Service.Interfaces;

namespace Vortex.Notification.Service.Providers
{
    public class LogEmailProvider(ILogger<LogEmailProvider> logger) : IEmailProvider
    {
        public Task SendEmailAsync(GenericEmail email)
        {
            logger.LogInformation("--- Mock Email Sent ---");
            logger.LogInformation("Recipient: {Recipient}", email.To);
            logger.LogInformation("Subject: {Subject}", email.Subject);
            logger.LogInformation("Body: {Body}", email.Body);
            logger.LogInformation("-----------------------");
            return Task.CompletedTask;
        }
    }
}
