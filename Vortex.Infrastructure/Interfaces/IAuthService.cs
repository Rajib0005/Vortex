using Vortex.Domain.Dto;

namespace Vortex.Infrastructure.Interfaces;

public interface IAuthService
{
    Task<string> GenerateTokenAsync(Guid userId, string email, CancellationToken cancellationToken = default);
    Task<string> SingUpAsync(AuthDto userModel, CancellationToken cancellationToken = default);
    Task<string> Login(AuthDto userModel, CancellationToken cancellationToken = default);
}