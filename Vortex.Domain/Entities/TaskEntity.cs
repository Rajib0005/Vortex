using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Vortex.Domain.Common;

namespace Vortex.Domain.Entities;

public class TaskEntity : IAuditable, ISupportParent, IProjectRelated
{
    public Guid Id { get; set; }
    public string? TaskName { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } =  TaskPriority.Medium;
    public Guid? ParentTaskId { get; set; } = null;
    public string Level { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
    public virtual ICollection<AttachmentEntity> Attachments { get; set; } = new List<AttachmentEntity>();
    public Guid ProjectId { get; set; }
    public virtual ProjectEntity Project { get; set; } = null!;

    public (string ParentType, Guid? ParentId) GetParentInfo()
    {
        return ("Project", ProjectId);
    }
}