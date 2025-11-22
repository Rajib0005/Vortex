using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Vortex.Domain.Constants;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Repositories;
using Vortex.Infrastructure.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Vortex.Application.Services;

public class AuthService() : IAuthService
{
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository;
    private readonly IGenericRepository<UserEntity>  _userRepository;
    private readonly IConfiguration _config;
    public AuthService(
        IGenericRepository<UserProjectRole> userProjectRoleRepository,
        IGenericRepository<UserEntity> userRepository,
        IConfiguration config) : this()
    {
        _userProjectRoleRepository = userProjectRoleRepository;
        _userRepository = userRepository;
        _config = config;
    }
    public async Task<string> GenerateTokenAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        var userProjectRole = await _userProjectRoleRepository
            .GetByCondition(u=> u.UserId == userId)
            .Include(u=> u.Role)
            .ToListAsync(cancellationToken);
        
        if (userProjectRole is null || userProjectRole.Count == 0)
            throw new Exception("Invalid username or password");
        
        var allAccessedProjects = userProjectRole.Select(userRole =>
        {
            var permisssions = (RolePermissionMap.RolePermissions?
                .GetValueOrDefault(userRole.Role.Name) ?? []).ToList();
            
            return new ProjectRolePermissionDto()
            {
                ProjectId = userRole.ProjectId.HasValue ? userRole.ProjectId.Value : Guid.Empty,
                RoleId = userRole.RoleId,
                Permission = permisssions
            };
        }).ToList();
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("project_access", JsonSerializer.Serialize(allAccessedProjects))
        };

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
        await DemandValidUser(userModel.Email, cancellationToken);
        
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

    public async Task<UserDetailsDto> GetUserDetailsByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository. GetByIdAsync(userId);
        
        if(existingUser is null) throw new Exception("User not found");

        var userDetails = new UserDetailsDto(
            existingUser.FullName, 
            existingUser.Email,
            existingUser.UserName,
            existingUser.IsActive,
            existingUser.EmailConfirmed
        );
        return userDetails;
    }

    private async Task DemandValidUser(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.
            GetByCondition(u=> u.Email == email).FirstOrDefaultAsync(cancellationToken);
        if (existingUser is not null)  throw new Exception("User already exists");
        
    }
}