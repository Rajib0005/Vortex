using Vortex.Application.Dtos;
using Vortex.Application.Dtos.Filtering;

namespace Vortex.Application.Interfaces;

public interface ITaskService
{
    Task CreateTaskAsync(UpsertTaskDto dto, CancellationToken cancellationToken = default);
    Task UpdateTaskAsync(Guid taskId, UpsertTaskDto dto, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task AssignTaskAsync(Guid taskId, Guid assigneeId, CancellationToken cancellationToken = default);
    Task<TaskDto?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<PagedResult<TaskDto>> GetFilteredTasksAsync(Guid projectId, TaskFilterQuery filter, CancellationToken cancellationToken = default);
}
