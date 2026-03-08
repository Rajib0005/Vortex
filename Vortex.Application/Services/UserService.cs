using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Exceptions;
using Vortex.Domain.Repositories;

namespace Vortex.Application.Services;

public class UserService(
    IHttpContextAccessor httpContextAccessor,
    IGenericRepository<UserEntity> userRepository,
    IGenericRepository<UserProjectRole> userProjectRoleRepository,
    IMapper mapper) : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IGenericRepository<UserEntity> _userRepository = userRepository;
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository = userProjectRoleRepository;
    private readonly IMapper _mapper = mapper;

    public Guid GetCurrentUserId()
    {
        var userClaim = _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userClaim != null ? userClaim.Value : string.Empty;
        return Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
    }

    public async Task<bool> IsExistingUser(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByCondition(u => u.Email == email)
            .AnyAsync(cancellationToken);
    }

    public async Task<IList<UserDetailsDto>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        // For GetAllUsers, we join with UserProjectRole to get the primary role
        return await _userRepository.GetByCondition(x => x.IsActive)
            .Include(x => x.Role)
            .ProjectTo<UserDetailsDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserDetailsDto> GetUserDetailsByIdAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        return await GetUserDetailsByIdAsync(currentUserId, cancellationToken);
    }

    public async Task<UserDetailsDto> GetUserDetailsByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userDetails = await _userRepository.GetByCondition(x => x.Id == userId)
            .Include(x => x.Role)
            .ProjectTo<UserDetailsDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (userDetails is null) throw new NotFoundException("User not found");

        return userDetails;
    }

    public async Task<List<UserToInviteInProject>> GetUserDetailsToInviteAsync(Guid? projectId, CancellationToken cancellation)
    {
        var usersWillBeInvited = new List<UserToInviteInProject>();
        var usersQuery = _userRepository.GetByCondition(x => true);

        if (projectId is not null)
        {
            usersQuery = usersQuery.Where(x => x.Projects.Any(y => y.ProjectId == projectId));
        }

        var users = await usersQuery.Take(20).ToListAsync(cancellation);

        var userOptions = users.Select(user => new UserToInviteInProject
        {
            UserId = user.Id,
            UserEmail = user.Email ?? string.Empty,
        });
        
        usersWillBeInvited.AddRange(userOptions);

        return usersWillBeInvited;
    }

    public async Task<List<string>> GetAlreadyExistingUsersInProject(List<string> userEmails, List<Guid> projectIds, CancellationToken cancellationToken)
    {
        var usersAlreadyInSameProject = await _userProjectRoleRepository.GetByCondition(user =>
            userEmails.Contains(user.User.Email!) && user.ProjectId.HasValue && projectIds.Contains(user.ProjectId.Value))
            .Select(x => x.User.Email)
            .ToListAsync(cancellationToken);

        return usersAlreadyInSameProject.Where(x => x is not null).Select(x => x!).ToList();
    }
}
