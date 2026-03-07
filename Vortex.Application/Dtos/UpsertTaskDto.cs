using Vortex.Domain;
using TaskStatus = Vortex.Domain.TaskStatus;

namespace Vortex.Application.Dtos;

public class UpsertTaskDto
{
    public required string TaskName { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskType TaskType { get; set; } = TaskType.Task;
    public int? StoryPoints { get; set; }
    
    public Guid ProjectId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? ReporterId { get; set; }
    
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    
    public int? OriginalEstimateMinutes { get; set; }
    public int? RemainingEstimateMinutes { get; set; }
    
    public string? Resolution { get; set; }
    public List<string> Labels { get; set; } = new();
}
