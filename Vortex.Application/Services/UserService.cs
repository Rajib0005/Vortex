using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Repositories;
using ProjectRoleDto = Vortex.Application.Dtos.ProjectRoleDto;

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

    public async Task<ProjectRoleDto> GetInviteUserDetails(CancellationToken cancellationToken)
    {
        var  projects = await _projectRepository.GetByCondition(proj=> proj.IsActive)
            .Select(x=> new DropdownOptionModel<Guid>{Value = x.Id, Label = x.ProjectName})
            .ToListAsync(cancellationToken);
        
        var roles = _roleRepository.GetAllAsync()
            .Select(r => new DropdownOptionModel<Guid> {Value = r.Id, Label = r.Name}).ToList();

        return new ProjectRoleDto {Projects = projects, Roles = roles};
    }

    public async Task InviteUserAsync(List<InviteUserDto> inviteUserDto, CancellationToken cancellationToken)
    {
        var userEmailsInModel = inviteUserDto.Select(x => x.UserEmail.ToLower()).ToList();
        var assignedProjects =inviteUserDto.Select(x=> x.ProjectId).ToList();
        var allReadyAssignedUsers = await GetAlreadyExistingUsersInProject(userEmailsInModel, assignedProjects, cancellationToken);
        var onlyInvitedUsers = inviteUserDto.Where(x => !allReadyAssignedUsers.Contains(x.UserEmail.ToLower()));
        
        var invitedUserDetails = onlyInvitedUsers.Select(user=>
        {
            var userId = Guid.NewGuid();
            return new UserProjectRole
            {
                User = new UserEntity
                {
                    Id = userId,
                    Email = user.UserEmail.ToLower(),
                    UserName = user.UserEmail.ToLower(),
                    EmailConfirmed = true,
                    IsActive = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("hello@123"),
                    CreatedOn = DateTime.UtcNow
                },
                UserId = userId,
                ProjectId = user.ProjectId,
                RoleId = user.RoleId,
            };
        }).ToList();
        
        await _userProjectRoleRepository.AddRangeAsync(invitedUserDetails);
        await _userProjectRoleRepository.SaveChangesAsync();
        
        //TODO: Email Service should be implemented
    }
    private async Task<List<string>> GetAlreadyExistingUsersInProject(List<string> userEmails, List<Guid> projectIds, CancellationToken cancellationToken)
    {
        var usersAlreadyInSameProject = await _userProjectRoleRepository.GetByCondition(user => 
            userEmails.Contains(user.User.Email) && projectIds.Contains(user.Project.Id)).Select(x=> x.User.Email).ToListAsync(cancellationToken);
        
        return usersAlreadyInSameProject;
    }
}