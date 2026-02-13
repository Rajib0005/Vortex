using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Constants;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Exceptions;
using Vortex.Domain.Repositories;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using Microsoft.AspNetCore.Identity;
using Vortex.Contracts.Models;

namespace Vortex.Application.Services;

public class AuthService(
    IGenericRepository<UserProjectRole> userProjectRoleRepository,
    IGenericRepository<UserEntity> userRepository,
    IUserService userService,
    UserManager<UserEntity> userManager,
    IBus bus,
    IProjectService projectService,
    IConfiguration config) : IAuthService
{
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository = userProjectRoleRepository;
    private readonly IGenericRepository<UserEntity> _userRepository = userRepository;
    private readonly IUserService _userService = userService;
    private readonly IConfiguration _config = config;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly IBus _bus = bus;
    private readonly IProjectService _projectService = projectService;

    public async Task<string> SingUpAsync(AuthDto userModel, CancellationToken cancellationToken)
    {
        var isExistingUser = await _userService.IsExistingUser(userModel.Email, cancellationToken);
        if (isExistingUser) throw new ConflictException("User already exists");

        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = userModel.Email,
            Email = userModel.Email,
            EmailConfirmed = true,
            IsActive = true,
            RoleId = Constants.AdminRoleId,
            CreatedOn = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(newUser, userModel.Password);

        if (!result.Succeeded)
        {
            throw new InternalServerException("Something went wrong!, failed to create user");
        }
        var projectUserRole = new UserProjectRole
        {
            Id = Guid.NewGuid(),
            UserId = newUser.Id,
            RoleId = Constants.AdminRoleId,
            ProjectId = Constants.DefaultProjectId,

        };

        await _userProjectRoleRepository.AddAsync(projectUserRole);
        await _userProjectRoleRepository.SaveChangesAsync();
        var secretKey = _config["JwtSettings:AuthenticationSecretKey"] ?? throw new InvalidOperationException("JWT secret key not found");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var token = await GenerateTokenAsync(newUser.Id, userModel.Email ?? string.Empty, key, cancellationToken);
        return token;
    }

    public async Task<string> Login(AuthDto userModel, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(userModel.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, userModel.Password))
        {
            throw new BadRequestException("Invalid username or password");
        }

        var secretKey = _config["JwtSettings:AuthenticationSecretKey"] ?? throw new InvalidOperationException("JWT secret key not found");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        return await GenerateTokenAsync(user.Id, user.Email ?? string.Empty, key, cancellationToken);
    }
    
    public async Task InviteUserAsync(List<InviteUserDto> inviteUserDto, CancellationToken cancellationToken)
    {
        var userEmailsInModel = inviteUserDto.Select(x => x.UserEmail.ToLower()).ToList();
        var assignedProjects =inviteUserDto.Select(x=> x.ProjectId).ToList();
        
        // Filter out users who are already active in the assigned projects
        var alreadyActiveUsersInProjects = await _userProjectRoleRepository
            .GetByCondition(user => userEmailsInModel.Contains(user.User.Email!) && 
                                   assignedProjects.Contains(user.Project.Id) &&
                                   user.User.IsActive)
            .Select(x => x.User.Email)
            .ToListAsync(cancellationToken);

        // Filter out users who are new or not active in the assigned projects
        var usersToInvite = inviteUserDto.Where(x => !alreadyActiveUsersInProjects.Contains(x.UserEmail.ToLower())).ToList();

        // Check for users who exist but are inactive and need a new invitation
        var existingInactiveUsers = await _userRepository
            .GetByCondition(u => userEmailsInModel.Contains(u.Email!) && !u.IsActive)
            .ToListAsync(cancellationToken);

        var userProjectRolesToAdd = new List<UserProjectRole>();
        var notificationsToSend = new List<NotificationContract>();

        foreach (var userDto in usersToInvite)
        {
            UserEntity userEntity;
            var existingInactiveUser = existingInactiveUsers.FirstOrDefault(u => string.Equals(u.Email, userDto.UserEmail.ToLower()));

            if (existingInactiveUser != null)
            {
                // User exists but is inactive, refresh invitation
                userEntity = existingInactiveUser;
                userEntity.IsActive = true;
            }
            else
            {
                // New user
                userEntity = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Email = userDto.UserEmail.ToLower(),
                    UserName = userDto.UserEmail.ToLower(),
                    IsActive = true,
                    EmailConfirmed = false,
                    RoleId = Constants.AdminRoleId,
                    CreatedOn = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(userEntity, Constants.DefaultUserPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InternalServerException($"Failed to create user: {errors}");
                }
                userProjectRolesToAdd.Add(new UserProjectRole
                {
                    User = userEntity,
                    UserId = userEntity.Id,
                    ProjectId = userDto.ProjectId,
                    RoleId = userDto.RoleId,
                });
                if (userProjectRolesToAdd.Count > 0)
                {
                    await _userProjectRoleRepository.AddRangeAsync(userProjectRolesToAdd);
                    await _userProjectRoleRepository.SaveChangesAsync();
                }
            }
            var secretKey = _config["JwtSettings:InvitationSecretKey"] ?? throw new InvalidOperationException("JWT secret key not found");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var invitationToken = await GenerateTokenAsync(userEntity.Id, userEntity.Email ?? string.Empty, key, cancellationToken);
            
            var invitationLink = $"{UrlConstants.BaseUrl}/set-password?token={invitationToken}";

            notificationsToSend.Add(new NotificationContract(
                NotificationId: Guid.NewGuid(),
                Destination: userEntity.Email ?? string.Empty,
                TemplateId: "InvitationEmail", // Matches the HTML file name without extension
                TemplateData: new Dictionary<string, string>
                {
                    { "Subject", "You have been invited to Vortex" },
                    { "InvitationLink", invitationLink }
                },
                Timestamp: DateTime.UtcNow
            ));
        }
        
        if (notificationsToSend.Any())
        {
            await _bus.PublishBatch(notificationsToSend, cancellationToken);
        }
    }

    public async Task SetPassword(SetPasswordDto setPasswordDto)
    {
        var principal = ValidateInvitationToken(setPasswordDto.Token)
            ?? throw new BadRequestException("Invalid or expired invitation token.");
        
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new BadRequestException("Invalid token payload: User ID not found.");

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found.");

        if (user.IsActive)
            throw new ConflictException("User already active. Password cannot be set via invitation link.");

        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!removePasswordResult.Succeeded)
            throw new InternalServerException($"Failed to remove existing password for user {user.Id}.");

        var addPasswordResult = await _userManager.AddPasswordAsync(user, setPasswordDto.NewPassword);
        if (!addPasswordResult.Succeeded)
            throw new BadRequestException($"Failed to set new password: {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");

        user.IsActive = true;
        user.EmailConfirmed = true;
        
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new InternalServerException($"Failed to activate user {user.Id} after setting password.");
    }
    
    private ClaimsPrincipal? ValidateInvitationToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _config["JwtSettings:InvitationSecretKey"] ?? throw new InvalidOperationException("JWT secret key not found");
        var key =  new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _config["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var principal = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
            return principal;
        }
        catch
        {
            // Token validation failed (e.g., expired, invalid signature)
            throw new UnauthorizedAccessException("Invalid invitation signature");
        }
    }

    #region private methods

    private async Task<string> GenerateTokenAsync(Guid userId, string email, SymmetricSecurityKey key, CancellationToken cancellationToken)
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

    #endregion
}