using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Vortex.Contracts.Emails;
using Vortex.Notification.Service.Interfaces;
using Vortex.Notification.Service.Models;

namespace Vortex.Notification.Service.Providers;

public class SmtpEmailProvider(IOptions<SmtpSettings> smtpSettings) : IEmailProvider
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public async Task SendEmailAsync(GenericEmail email)
    {
        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Vortex", _smtpSettings.From));
            mimeMessage.To.Add(new MailboxAddress(email.To, email.To));
            mimeMessage.Subject = email.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = email.Body };
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occured while sending email", ex);
        }
    }
}
