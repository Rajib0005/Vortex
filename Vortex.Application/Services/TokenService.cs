using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Repositories;
using Vortex.Infrastructure.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Vortex.Application.Services;

public class TokenService: ITokenService
{
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository;
    private readonly IConfiguration _config;
    public TokenService(
        IGenericRepository<UserProjectRole> userProjectRoleRepository,
        IConfiguration config)
    {
        _userProjectRoleRepository = userProjectRoleRepository;
        _config = config;
    }

    public string GenerateTokenAsync(AuthDto userModel)
    {
        var user =  _userProjectRoleRepository.
            GetByCondition(u=> u.User.Email == userModel.Email).FirstOrDefault();
        
        if (user is null || BCrypt.Net.BCrypt.Verify(user.User.PasswordHash, userModel.Password))
            throw new Exception("Invalid username or password");
        
        var accessedProjects = _userProjectRoleRepository
            .GetByCondition(x => x.UserId == user.Id)
            .Select(pr=> new
            {
                projectId = pr.ProjectId,
                roleId = pr.RoleId,
            });
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.User.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userModel.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Vortex-Token-Key"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}