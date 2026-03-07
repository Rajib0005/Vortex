using Microsoft.AspNetCore.Mvc;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;

namespace Vortex.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuditLogController(IAuditLogService auditLogService) : ControllerBase
{
    private readonly IAuditLogService _auditLogService = auditLogService;

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetAuditLogsByProject(Guid projectId, CancellationToken ct)
    {
        var logs = await _auditLogService.GetAuditLogsByProjectAsync(projectId, ct);
        return Ok(BaseResponse<IEnumerable<object>>.SuccessResponse(logs));
    }

    [HttpGet("task/{entityId}")]
    public async Task<IActionResult> GetAuditLogsByTask(Guid entityId, CancellationToken ct)
    {
        var logs = await _auditLogService.GetAuditLogsByTaskAsync(entityId, ct);
        return Ok(BaseResponse<IEnumerable<object>>.SuccessResponse(logs));
    }
}
