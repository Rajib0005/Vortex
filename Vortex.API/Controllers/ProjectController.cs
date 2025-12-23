using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;

namespace Vortex.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController>  _logger;
    public ProjectController(ILogger<ProjectController> logger, IProjectService projectService)
    {
        _logger = logger;
        _projectService = projectService;
    }
    
    [HttpPost("upsert-project")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> CreateOrUpdateProject([FromBody] UpsertProjectDto projectModel, CancellationToken cancellation)
    {
        try
        {
            await _projectService.UpsertProjectAsync(projectModel, cancellation);
            return Ok(BaseResponse<string>.SuccessResponse(
                null,
                $"Project {(projectModel.ProjectId is null ? "created" : "updated")} successfully"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, BaseResponse<Exception>.FailureResponse("Error creating project", [ex.Message]));
        }
    }
    
    [HttpPost("get-projects")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> GetAllProjectsForUser(Guid userId, CancellationToken cancellation)
    {
        try
        {
            var projects = await _projectService.GetProjectsOfUser(userId, cancellation);
            return Ok(BaseResponse<IEnumerable<ProjectCardsDto>>.SuccessResponse(projects));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, BaseResponse<Exception>.FailureResponse("Error retrieving project", [ex.Message]));
        }
    }
}