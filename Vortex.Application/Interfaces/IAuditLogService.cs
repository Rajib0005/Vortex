using Vortex.Application.Dtos;

namespace Vortex.Application.Interfaces;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogDto>> GetAuditLogsByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<IEnumerable<AuditLogDto>> GetAuditLogsByTaskAsync(Guid entityId, CancellationToken ct = default);
}
