using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Entities;
using Vortex.Domain.Exceptions;
using Vortex.Domain.Repositories;
using TaskStatus = Vortex.Domain.TaskStatus;

namespace Vortex.Application.Services;

public class TaskService(
    IGenericRepository<TaskEntity> taskRepository,
    IGenericRepository<ProjectEntity> projectRepository,
    ICurrentUserService currentUserService) : ITaskService
{
    private readonly IGenericRepository<TaskEntity> _taskRepository = taskRepository;
    private readonly IGenericRepository<ProjectEntity> _projectRepository = projectRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task CreateTaskAsync(UpsertTaskDto dto, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(dto.ProjectId) 
            ?? throw new BadRequestException("Project not found");

        project.LastTaskSequence++;
        var taskKey = $"{project.ProjectKey}-{project.LastTaskSequence}";

        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            ProjectId = dto.ProjectId,
            TaskName = dto.TaskName,
            TaskKey = taskKey,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = dto.Status,
            TaskType = dto.TaskType,
            StoryPoints = dto.StoryPoints,
            ParentTaskId = dto.ParentTaskId,
            AssigneeId = dto.AssigneeId,
            ReporterId = dto.ReporterId ?? _currentUserService.UserId,
            StartDate = dto.StartDate ?? DateTime.UtcNow,
            DueDate = dto.DueDate,
            OriginalEstimateMinutes = dto.OriginalEstimateMinutes,
            RemainingEstimateMinutes = dto.RemainingEstimateMinutes ?? dto.OriginalEstimateMinutes,
            Labels = dto.Labels,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId,
            UpdatedBy = _currentUserService.UserId
        };

        await _taskRepository.AddAsync(task);
        _projectRepository.UpdateAsync(project);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(UpsertTaskDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Id == null) throw new Exception("Task Id is required for update");
        
        var task = await _taskRepository.GetByIdAsync(dto.Id.Value) 
            ?? throw new BadRequestException("Task not found");

        task.TaskName = dto.TaskName;
        task.Description = dto.Description;
        task.Priority = dto.Priority;
        task.Status = dto.Status;
        task.TaskType = dto.TaskType;
        task.StoryPoints = dto.StoryPoints;
        task.ParentTaskId = dto.ParentTaskId;
        task.AssigneeId = dto.AssigneeId;
        task.ReporterId = dto.ReporterId ?? task.ReporterId;
        task.StartDate = dto.StartDate ?? task.StartDate;
        task.DueDate = dto.DueDate;
        task.OriginalEstimateMinutes = dto.OriginalEstimateMinutes;
        task.RemainingEstimateMinutes = dto.RemainingEstimateMinutes;
        task.Labels = dto.Labels;
        task.Resolution = dto.Resolution;
        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedBy = _currentUserService.UserId;

        if (task.Status == TaskStatus.Done && task.CompletedAt == null)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (task.Status != TaskStatus.Done)
        {
            task.CompletedAt = null;
        }

        _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId) 
            ?? throw new BadRequestException("Task not found");
        
        _taskRepository.DeleteAsync(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task AssignTaskAsync(Guid taskId, Guid assigneeId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId) 
            ?? throw new BadRequestException("Task not found");
        
        task.AssigneeId = assigneeId;
        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedBy = _currentUserService.UserId;
        
        _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task<TaskDto?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByCondition(x => x.Id == taskId)
            .Include(x => x.Assignee)
            .Include(x => x.Reporter)
            .Include(x => x.Attachments)
            .Include(x => x.Comments)
            .Select(x => MapToDto(x))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByCondition(x => x.ProjectId == projectId)
            .Include(x => x.Assignee)
            .Include(x => x.Reporter)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);
    }

    private static TaskDto MapToDto(TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            TaskKey = task.TaskKey,
            TaskName = task.TaskName,
            Description = task.Description,
            Level = task.Level,
            TaskType = task.TaskType,
            Status = task.Status,
            Priority = task.Priority,
            Resolution = task.Resolution,
            Labels = task.Labels,
            StoryPoints = task.StoryPoints,
            OriginalEstimateMinutes = task.OriginalEstimateMinutes,
            RemainingEstimateMinutes = task.RemainingEstimateMinutes,
            TimeSpentMinutes = task.TimeSpentMinutes,
            ProjectId = task.ProjectId,
            ParentTaskId = task.ParentTaskId,
            Assignee = task.Assignee != null ? new UserSummaryDto
            {
                Id = task.Assignee.Id,
                Name = task.Assignee.UserName ?? string.Empty,
                Email = task.Assignee.Email,
                AvatarUrl = null // Placeholder
            } : null,
            Reporter = task.Reporter != null ? new UserSummaryDto
            {
                Id = task.Reporter.Id,
                Name = task.Reporter.UserName ?? string.Empty,
                Email = task.Reporter.Email,
                AvatarUrl = null // Placeholder
            } : null,
            StartDate = task.StartDate,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            CommentCount = task.Comments.Count,
            AttachmentCount = task.Attachments.Count
        };
    }
}
