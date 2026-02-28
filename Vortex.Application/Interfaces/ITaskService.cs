using Vortex.Application.Dtos;

namespace Vortex.Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateTaskAsync(UpsertTaskDto dto, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateTaskAsync(UpsertTaskDto dto, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task AssignTaskAsync(Guid taskId, Guid assigneeId, CancellationToken cancellationToken = default);
    Task<TaskDto?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
}
