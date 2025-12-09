using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Vortex.Application.Interfaces;
using Vortex.Domain.Constants;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Repositories;
using Vortex.Infrastructure.CustomException;
using Vortex.Infrastructure.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Vortex.Application.Services;

public class AuthService() : IAuthService
{
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository;
    private readonly IGenericRepository<UserEntity> _userRepository;
    private readonly IGenericRepository<ProjectEntity> _projectRepository;
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public AuthService(
        IGenericRepository<UserProjectRole> userProjectRoleRepository,
        IGenericRepository<UserEntity> userRepository,
        IGenericRepository<ProjectEntity> projectRepository,
        IUserService userService,
        IConfiguration config) : this()
    {
        _userProjectRoleRepository = userProjectRoleRepository;
        _userRepository = userRepository;
        _userService = userService;
        _projectRepository = projectRepository;
        _config = config;
    }

    public async Task<string> GenerateTokenAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        var userProjectRole = await _userProjectRoleRepository
            .GetByCondition(u => u.UserId == userId)
            .Include(u => u.Role)
            .ToListAsync(cancellationToken);

        if (userProjectRole is null || userProjectRole.Count == 0)
            throw new BadRequestException("Invalid username or password");

        var allAccessedProjects = userProjectRole.Select(userRole =>
        {
            var permissions = (RolePermissionMap.RolePermissions[userRole.RoleId] ?? []).ToList();

            return new ProjectRolePermissionDto()
            {
                ProjectId = userRole.ProjectId ?? Guid.Empty,
                RoleId = userRole.RoleId,
                Permission = permissions
            };
        }).ToList();
        
        var rolesClaim = userProjectRole.Select(role => new Claim(ClaimTypes.Role, role.Role.Name)).ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("project_access", JsonSerializer.Serialize(allAccessedProjects)),
        }.Concat(rolesClaim).ToList();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:AuthenticationSecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> SingUpAsync(AuthDto userModel, CancellationToken cancellationToken)
    {
        var isExistingUser = await _userService.IsExistingUser(userModel.Email, cancellationToken);
        if (isExistingUser) throw new ConflictException("User already exists");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
        var newUser = new UserEntity()
        {
            Id = Guid.NewGuid(),
            UserName = userModel.Email,
            Email = userModel.Email,
            EmailConfirmed = true,
            IsActive = true,
            PasswordHash = hashedPassword,
            CreatedOn = DateTime.UtcNow
        };

        await _userRepository.AddAsync(newUser);


        var projectUserRole = new UserProjectRole
        {
            Id = Guid.NewGuid(),
            UserId = newUser.Id,
            RoleId = Constants.AdminRoleId,
            ProjectId = Constants.DefaultProjectId,

        };

        await _userProjectRoleRepository.AddAsync(projectUserRole);

        await _userRepository.SaveChangesAsync();
        await _userProjectRoleRepository.SaveChangesAsync();

        var token = await GenerateTokenAsync(newUser.Id, userModel.Email, cancellationToken);
        return token;
    }

    public async Task<string> Login(AuthDto userModel, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByCondition(u => u.Email == userModel.Email)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(userModel.Password, user.PasswordHash, false, HashType.SHA256))
            throw new BadRequestException("Invalid username or password");
        return await GenerateTokenAsync(user.Id, user.Email, cancellationToken);
    }

    public async Task<UserDetailsDto> GetUserDetailsByIdAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetCurrentUserId();
        var existingUser = await _userRepository.GetByIdAsync(currentUserId);

        if (existingUser is null) throw new NotFoundException("User not found");

        var userDetails = new UserDetailsDto(
            existingUser.FullName,
            existingUser.Email,
            existingUser.UserName,
            existingUser.IsActive,
            existingUser.EmailConfirmed
        );
        return userDetails;
    }
}