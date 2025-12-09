namespace Vortex.API.Dtos;

public class InviteUserDto
{
    public string UserEmail { get; set; }
    public Guid ProjectId { get; set; }
    public Guid RoleId { get; set; }
}