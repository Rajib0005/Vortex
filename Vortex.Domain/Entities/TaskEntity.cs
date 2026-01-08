using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Vortex.Domain.Entities;

public class TaskEntity
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
    public virtual ProjectEntity Project { get; set; }
}