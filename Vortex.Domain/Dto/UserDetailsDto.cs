namespace Vortex.Domain.Dto;

public class UserDetailsDto(Guid userId, string fullName, string email, string userName, bool isActive, bool isEmailConfirmed, Guid roleId, string roleName)
{
    public Guid Id {get; set;} = userId;
    public string? FullName { get; set; } = fullName;
    public string? Email { get; set; } = email;
    public string? UserName { get; set; } = userName;
    public bool IsActive { get; set; } = isActive;
    public bool IsEmailConfirmed { get; set; } = isEmailConfirmed;
    public Guid RoleId { get; set; } = roleId;
    public string RoleName { get; set; } =  roleName;
}