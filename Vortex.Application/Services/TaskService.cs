using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Dtos.Filtering;
using Vortex.Application.Interfaces;
using Vortex.Domain.Entities;
using Vortex.Domain.Exceptions;
using Vortex.Domain.Repositories;
using TaskStatus = Vortex.Domain.TaskStatus;

namespace Vortex.Application.Services;

public class TaskService(
    IGenericRepository<TaskEntity> taskRepository,
    IGenericRepository<ProjectEntity> projectRepository,
    ICurrentUserService currentUserService,
    IFilteringService filteringService,
    IMapper mapper) : ITaskService
{
    private readonly IGenericRepository<TaskEntity> _taskRepository = taskRepository;
    private readonly IGenericRepository<ProjectEntity> _projectRepository = projectRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IFilteringService _filteringService = filteringService;
    private readonly IMapper _mapper = mapper;

    public async Task CreateTaskAsync(UpsertTaskDto dto, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(dto.ProjectId) 
            ?? throw new BadRequestException("Project not found");

        project.LastTaskSequence++;
        var taskKey = $"{project.ProjectKey}-{project.LastTaskSequence}";

        var task = _mapper.Map<TaskEntity>(dto);
        task.Id = Guid.NewGuid();
        task.TaskKey = taskKey;
        task.ReporterId = dto.ReporterId ?? _currentUserService.UserId;
        task.StartDate = dto.StartDate ?? DateTime.UtcNow;
        task.RemainingEstimateMinutes = dto.RemainingEstimateMinutes ?? dto.OriginalEstimateMinutes;
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        task.CreatedBy = _currentUserService.UserId;
        task.UpdatedBy = _currentUserService.UserId;

        await _taskRepository.AddAsync(task);
        _projectRepository.UpdateAsync(project);
        await _taskRepository.SaveChangesAsync();

    }

    public async Task UpdateTaskAsync(Guid taskId, UpsertTaskDto dto, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId) 
            ?? throw new BadRequestException("Task not found");

        _mapper.Map(dto, task);

        task.ReporterId = dto.ReporterId ?? task.ReporterId;
        task.StartDate = dto.StartDate ?? task.StartDate;
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
            .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByCondition(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<TaskDto>> GetFilteredTasksAsync(
        Guid projectId,
        TaskFilterQuery filter,
        CancellationToken cancellationToken = default)
    {
        // Build a safe, immutable filter with ProjectId enforced from the route
        var safeFilter = new TaskFilterQueryBuilder()
            .ForProject(projectId)
            .WithStatuses(filter.Statuses.ToArray())
            .WithPriorities(filter.Priorities.ToArray())
            .WithTaskTypes(filter.TaskTypes.ToArray())
            .WithAssignees(filter.AssigneeIds.ToArray())
            .WithReporters(filter.ReporterIds.ToArray())
            .WithLabels(filter.Labels.ToArray())
            .Search(filter.SearchTerm)
            .DueBetween(filter.DueDateFrom, filter.DueDateTo)
            .StartedBetween(filter.StartDateFrom, filter.StartDateTo)
            .CreatedBetween(filter.CreatedFrom, filter.CreatedTo)
            .OnPage(filter.Page, filter.PageSize)
            .SortBy(filter.SortBy, filter.SortDesc)
            .Build();

        var source = _taskRepository.GetByCondition(_ => true);
        return await _filteringService.GetFilteredAsync<TaskEntity, TaskFilterQuery, TaskDto>(
            source, safeFilter, new TaskFilterSpecification(), cancellationToken);
    }
}