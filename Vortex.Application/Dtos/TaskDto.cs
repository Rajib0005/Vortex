using Vortex.Domain;
using TaskStatus = Vortex.Domain.TaskStatus;

namespace Vortex.Application.Dtos;

public class TaskDto
{
    // Core Info
    public Guid Id { get; set; }
    public string TaskKey { get; set; } = string.Empty;
    public string? TaskName { get; set; }
    public string? Description { get; set; }
    public string Level { get; set; } = string.Empty;
    
    // Classifications
    public TaskType TaskType { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public string? Resolution { get; set; }
    public List<string> Labels { get; set; } = new();

    // Agile & Time Tracking
    public int? StoryPoints { get; set; }
    public int? OriginalEstimateMinutes { get; set; } 
    public int? RemainingEstimateMinutes { get; set; }
    public int? TimeSpentMinutes { get; set; }

    // Relationships
    public Guid ProjectId { get; set; }
    public Guid? ParentTaskId { get; set; }
    
    public UserSummaryDto? Assignee { get; set; }
    public UserSummaryDto? Reporter { get; set; }

    // Dates
    public DateTime StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Metadata / Counters
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }
}
