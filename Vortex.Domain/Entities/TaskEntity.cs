using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using Vortex.Domain.Common;

namespace Vortex.Domain.Entities;

public class TaskEntity : IAuditable, ISupportParent, IProjectRelated
{
    public Guid Id { get; set; }
    public string? TaskName { get; set; }
    public string TaskKey { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } =  TaskPriority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public int? StoryPoints { get; set; }
    public Guid? ParentTaskId { get; set; } = null;
    public string Level { get; set; } = string.Empty;
    
    public Guid? AssigneeId { get; set; }
    [ForeignKey("AssigneeId")]
    public virtual UserEntity? Assignee { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
    public virtual ICollection<AttachmentEntity> Attachments { get; set; } = new List<AttachmentEntity>();
    public virtual ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
    public Guid ProjectId { get; set; }
    public virtual ProjectEntity Project { get; set; } = null!;
    public TaskType TaskType { get; set; } = TaskType.Task;
    
    // Time Tracking
    public int? OriginalEstimateMinutes { get; set; }
    public int? RemainingEstimateMinutes { get; set; }
    public int? TimeSpentMinutes { get; set; }
    
    // Reporter
    public Guid? ReporterId { get; set; }
    [ForeignKey("ReporterId")]
    public virtual UserEntity? Reporter { get; set; }
    
    // Categorization
    public string? Resolution { get; set; }
    public List<string> Labels { get; set; } = new List<string>();
    public (string ParentType, Guid? ParentId) GetParentInfo()
    {
        return ("Project", ProjectId);
    }
}