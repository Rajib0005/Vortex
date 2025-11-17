namespace Vortex.Domain.Dto;

public class ProjectRoleDto
{
    public Guid ProjectId { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}