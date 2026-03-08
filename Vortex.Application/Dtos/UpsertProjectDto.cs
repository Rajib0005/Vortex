using Vortex.Domain;

namespace Vortex.Application.Dtos;

public class UpsertProjectDto
{
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectDescription { get; set; }
    public string? ProjectKey { get; set; }
    public bool? IsActive { get; set; }
    public ProjectPriority? Priority { get; set; }
    public DateTime? EstimatedDeadline { get; set; }
    public string? Domain { get; set; }
    public List<UserToInviteInProject> InviteUsers { get; set; } = [];
}