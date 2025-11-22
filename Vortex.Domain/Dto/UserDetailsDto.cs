namespace Vortex.Domain.Dto;

public class UserDetailsDto(string fullName, string email, string userName, bool isActive, bool isEmailConfimed)
{
    public string FullName { get; set; } = fullName;
    public string Email { get; set; } = email;
    public string UserName { get; set; } = userName;
    public bool IsActive { get; set; } = isActive;
    public bool IsEmailConfirmed { get; set; } = isEmailConfimed;
}