using MassTransit;
using Microsoft.Extensions.Logging;
using Vortex.Contracts;

namespace Vortex.Notification.Service.Consumers;

public class SendInvitationEmailConsumer : IConsumer<SendInvitationEmail>
{
    private readonly ILogger<SendInvitationEmailConsumer> _logger;

    public SendInvitationEmailConsumer(ILogger<SendInvitationEmailConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<SendInvitationEmail> context)
    {
        _logger.LogInformation(
            "Sending invitation email to {ToEmail} with link: {InvitationLink}",
            context.Message.ToEmail,
            context.Message.InvitationLink);

        // TODO: Add actual email sending logic here (e.g., using a third-party email service)
        
        return Task.CompletedTask;
    }
}
