using System.Threading.Tasks;
using Vortex.Contracts.Emails;

namespace Vortex.Notification.Service.Interfaces
{
    public interface IEmailProvider
    {
        Task SendEmailAsync(GenericEmail email);
    }
}
