using Vortex.Application.Dtos;

namespace Vortex.Application.Interfaces;

public interface IUserService
{
    Guid GetCurrentUserId();
    public Task<bool> IsExistingUser(string email, CancellationToken cancellationToken);
    public Task<ProjectRoleDto> GetInviteUserDetails(CancellationToken cancellationToken);
    public Task InviteUserAsync(List<InviteUserDto> inviteUserDto, CancellationToken cancellationToken);
}