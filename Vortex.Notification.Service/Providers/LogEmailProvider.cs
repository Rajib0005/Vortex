using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vortex.Notification.Service.Interfaces;

namespace Vortex.Notification.Service.Providers
{
    public class LogEmailProvider : IEmailProvider
    {
        private readonly ILogger<LogEmailProvider> _logger;

        public LogEmailProvider(ILogger<LogEmailProvider> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string recipient, string subject, string body)
        {
            _logger.LogInformation("--- Mock Email Sent ---");
            _logger.LogInformation("Recipient: {Recipient}", recipient);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {Body}", body);
            _logger.LogInformation("-----------------------");
            return Task.CompletedTask;
        }
    }
}
