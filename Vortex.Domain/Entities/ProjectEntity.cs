using System.ComponentModel.DataAnnotations;

namespace Vortex.Domain.Entities;

public class ProjectEntity
{
    [Key]
    public required Guid Id { get; set; }
    public required string ProjectName { get; set; }
    public string? Description { get; set; } = null;
    public required string ProjectKey { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}