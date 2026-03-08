using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Exceptions;
using Vortex.Domain.Repositories;

namespace Vortex.Application.Services;

public class UserService: IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<UserEntity>  _userRepository;
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository;
    public UserService(
        IHttpContextAccessor httpContextAccessor
        , IGenericRepository<UserEntity>  userRepository
        , IGenericRepository<UserProjectRole> userProjectRoleRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
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

    public async Task<IList<UserDetailsDto>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByCondition(x => x.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var userIds = users.Select(u => u.Id).ToList();

        var userRoles = await _userProjectRoleRepository.GetByCondition(upr => userIds.Contains(upr.UserId))
            .Include(upr => upr.Role)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var userDetailsList = new List<UserDetailsDto>();

        foreach (var user in users)
        {
            var primaryRole = userRoles.FirstOrDefault(ur => ur.UserId == user.Id)?.Role;

            if (primaryRole != null)
            {
                userDetailsList.Add(new UserDetailsDto(
                    user.Id,
                    user.FullName ?? string.Empty,
                    user.Email ?? string.Empty,
                    user.UserName ?? string.Empty,
                    user.IsActive,
                    user.EmailConfirmed,
                    primaryRole.Id,
                    primaryRole.Name ?? string.Empty
                ));
            }
        }

        return userDetailsList;
    }

    public async Task<UserDetailsDto> GetUserDetailsByIdAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        return await GetUserDetailsByIdAsync(currentUserId, cancellationToken);
    }

    public async Task<UserDetailsDto> GetUserDetailsByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId);
        var role = await _userProjectRoleRepository
            .GetByCondition(x => x.UserId == userId)
            .Select(x => x.Role)
            .FirstOrDefaultAsync(cancellationToken);
        var userDetails = new UserDetailsDto(
            existingUser.Id,
            existingUser.FullName ?? string.Empty,
            existingUser.Email ?? string.Empty,
            existingUser.UserName ?? string.Empty,
            existingUser.IsActive,
            existingUser.EmailConfirmed,
            role.Id,
            role.Name ?? string.Empty
        );

        if (userDetails is not null) return userDetails;

        throw new NotFoundException("User not found");
    }

    public async Task<List<UserToInviteInProject>> GetUserDetailsToInviteAsync(Guid? projectId, CancellationToken cancellation)
    {
        var usersWillBeInvited = new List<UserToInviteInProject>();
        var users = await _userRepository.GetAllAsync(cancellation);

        if (projectId is not null)
        {
            users = users.Where(x=> x.Projects.Any(y => y.ProjectId == projectId))
                .Take(20)
                .Skip(1);
        }

        var userOptions = users
                .Select<UserEntity, UserToInviteInProject>(user => new UserToInviteInProject
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                });
            usersWillBeInvited.AddRange(userOptions);
        
        return usersWillBeInvited;
    }
    public  async Task<List<string>> GetAlreadyExistingUsersInProject(List<string> userEmails, List<Guid> projectIds, CancellationToken cancellationToken)
    {
        var usersAlreadyInSameProject = await _userProjectRoleRepository.GetByCondition(user => 
            userEmails.Contains(user.User.Email!) && projectIds.Contains(user.Project.Id)).Select(x=> x.User.Email).ToListAsync(cancellationToken);
        
        return usersAlreadyInSameProject.Where(x => x is not null).Select(x => x!).ToList();
    }
}