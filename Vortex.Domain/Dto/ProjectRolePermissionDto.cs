namespace Vortex.Domain.Dto;

public class ProjectRolePermissionDto
{
    public Guid ProjectId { get; set; }
    public Guid RoleId { get; set; }
    public List<string> Permission { get; set; } = new List<string>();
}