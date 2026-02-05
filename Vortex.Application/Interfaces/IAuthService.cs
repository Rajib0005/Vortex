using Microsoft.IdentityModel.Tokens;
using Vortex.Application.Dtos;
using Vortex.Domain.Dto;

namespace Vortex.Application.Interfaces;

public interface IAuthService
{
    Task<string> SingUpAsync(AuthDto userModel, CancellationToken cancellationToken = default);
    Task<string> Login(AuthDto userModel, CancellationToken cancellationToken = default);
    Task InviteUserAsync(List<InviteUserDto> inviteUserDto, CancellationToken cancellationToken);
    Task SetPassword(SetPasswordDto setPasswordDto);
}
