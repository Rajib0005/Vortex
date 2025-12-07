using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Repositories;

namespace Vortex.Application.Services;

public class UserService: IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<UserEntity>  _userRepository;
    private readonly IGenericRepository<RoleEntity> _roleRepository;
    private readonly IGenericRepository<ProjectEntity> _projectRepository;
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository;
    public UserService(
        IHttpContextAccessor httpContextAccessor
        , IGenericRepository<UserEntity>  userRepository
        , IGenericRepository<RoleEntity>  roleRepository
        , IGenericRepository<ProjectEntity> projectRepository,
        IGenericRepository<UserProjectRole> userProjectRoleRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _projectRepository = projectRepository;
        _userProjectRoleRepository = userProjectRoleRepository;
    }
    public Guid GetCurrentUserId()
    {
        var userClaim = _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userClaim != null ? userClaim.Value : string.Empty;
        return Guid.Parse(userId);
    }
    
    public async Task<bool> IsExistingUser(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByCondition(u => u.Email == email).
            FirstOrDefaultAsync(cancellationToken);
        return existingUser is not null;
    }

    public async Task<InviteUserDetails> GetInviteUserDetails(CancellationToken cancellationToken)
    {
        // TODO: Other user cannot be invited.
        
        var roles = _roleRepository.GetAllAsync()
            .Select(r => new DropdownOptionModel<Guid>
            {
                Value = r.Id,
                Label = r.Name,
            }).ToList();
        var activeProjectUserDetails = await _userProjectRoleRepository
            .GetByCondition(urp => urp.Project.IsActive)
            .GroupBy(urp => urp.ProjectId)
            .Select(project => new InviteUserDetails
            {
                Projects = project.Select(urpd => new DropdownOptionModel<Guid?, List<DropdownOptionModel<Guid>>>
                {
                    Value = urpd.ProjectId,
                    Label = urpd.Project.ProjectName,
                    ExtraData = new List<DropdownOptionModel<Guid>>
                    {
                       new() {Value = urpd.UserId, Label = urpd.User.UserName}
                    }

                }),
                Roles = roles
            }).FirstOrDefaultAsync(cancellationToken);
        return activeProjectUserDetails;
    }

    public async Task InviteUserAsync(InviteUserDetails inviteUserDetails, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}