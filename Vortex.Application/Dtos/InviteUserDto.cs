namespace Vortex.API.Dtos;

public class InviteUserDto
{
    public Guid UserId { get; set; }
    public List<Guid> ProjectIds { get; set; }
    public Guid RoleId { get; set; }
}