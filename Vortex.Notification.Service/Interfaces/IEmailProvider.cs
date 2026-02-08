using System.Threading.Tasks;

namespace Vortex.Notification.Service.Interfaces
{
    public interface IEmailProvider
    {
        Task SendEmailAsync(string recipient, string subject, string body);
    }
}
