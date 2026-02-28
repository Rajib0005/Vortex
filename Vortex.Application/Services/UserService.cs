using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
}
