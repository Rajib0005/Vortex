using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Entities;
using Vortex.Domain.Repositories;

namespace Vortex.Application.Services;

public class AuditLogService(
    IGenericRepository<AuditLog> auditLogRepository,
    IMapper mapper) : IAuditLogService
{
    private readonly IGenericRepository<AuditLog> _auditLogRepository = auditLogRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _auditLogRepository.GetByCondition(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.DateTime)
            .ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByTaskAsync(Guid entityId, CancellationToken ct = default)
    {
        return await _auditLogRepository.GetByCondition(x => x.EntityId == entityId || x.ParentEntityId == entityId)
            .OrderByDescending(x => x.DateTime)
            .ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
