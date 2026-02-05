namespace Vortex.Application.Dtos;

public class InviteUserDto
{
    public required string UserEmail { get; set; }
    public Guid ProjectId { get; set; }
    public Guid RoleId { get; set; }
}