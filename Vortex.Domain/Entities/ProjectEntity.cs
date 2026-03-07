using System.ComponentModel.DataAnnotations;

using Vortex.Domain.Common;

namespace Vortex.Domain.Entities;

public class ProjectEntity : IAuditable, IProjectRelated
{
    [Key]
    public required Guid Id { get; set; }
    public required string ProjectName { get; set; }
    public string? Description { get; set; } = null;
    public required string ProjectKey { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
    public int LastTaskSequence { get; set; } = 0;
    public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public Guid ProjectId => Id;
}