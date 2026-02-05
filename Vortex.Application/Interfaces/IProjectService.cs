using Vortex.Application.Dtos;

namespace Vortex.Application.Interfaces;

public interface IProjectService
{
    Task UpsertProjectAsync(UpsertProjectDto project, CancellationToken ct = default, Guid? createdBy = null);
    Task DeleteProject(Guid projectId, CancellationToken ct = default);
    Task<IEnumerable<ProjectCardsDto>> GetProjectsOfUser(Guid userId, CancellationToken ct = default);
    Task<ProjectCardsDto> GetProjectDetailsById(Guid projectId);
}