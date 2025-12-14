using Vortex.Application.Dtos;
using Vortex.Domain.Dto;
using ProjectRoleDto = Vortex.Application.Dtos.ProjectRoleDto;

namespace Vortex.Application.Interfaces;

public interface IUserService
{
    public Task<UserDetailsDto> GetUserDetailsByIdAsync(CancellationToken cancellationToken = default);
    public Task<bool> IsExistingUser(string email, CancellationToken cancellationToken);
    public Task<ProjectRoleDto> GetInviteUserDetails(CancellationToken cancellationToken);
    public Task InviteUserAsync(List<InviteUserDto> inviteUserDto, CancellationToken cancellationToken);
}