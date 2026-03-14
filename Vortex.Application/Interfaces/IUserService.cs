using Vortex.Application.Dtos;
using Vortex.Domain.Dto;

namespace Vortex.Application.Interfaces;

public interface IUserService
{
    public Guid GetCurrentUserId();
    Guid GetCurrentUserRoleId();
    public Task<UserDetailsDto> GetUserDetailsByIdAsync(CancellationToken cancellationToken = default);
    public Task<UserDetailsDto> GetUserDetailsByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<bool> IsExistingUser(string email, CancellationToken cancellationToken);
    public Task<IList<UserDetailsDto>> GetAllUsers(CancellationToken cancellationToken = default);

    Task<List<UserToInviteInProject>> GetUserDetailsToInviteAsync(Guid? projectId, CancellationToken cancellation);
}