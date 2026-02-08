using Vortex.Domain.Dto;
using ProjectRoleDto = Vortex.Application.Dtos.ProjectRoleDto;

namespace Vortex.Application.Interfaces;

public interface IUserService
{
    public Guid GetCurrentUserId();
    public Task<UserDetailsDto> GetUserDetailsByIdAsync(CancellationToken cancellationToken = default);
    public Task<UserDetailsDto> GetUserDetailsByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<bool> IsExistingUser(string email, CancellationToken cancellationToken);
    public Task<IList<UserDetailsDto>> GetAllUsers(CancellationToken cancellationToken = default);
}