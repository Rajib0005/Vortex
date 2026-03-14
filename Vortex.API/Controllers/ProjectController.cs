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
    private readonly IUserService _userService; 
    public ProjectController(ILogger<ProjectController> logger, IProjectService projectService, IUserService userService)
    {
        _logger = logger;
        _projectService = projectService;
        _userService = userService;
    }
    
    [HttpPost("upsert-project")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> CreateOrUpdateProject([FromBody] UpsertProjectDto projectModel, CancellationToken cancellation)
    {
        try
        {
            await _projectService.UpsertProjectAsync(projectModel, cancellation);
            return Ok(BaseResponse<string>.SuccessResponse($"Project {(projectModel.ProjectId is null ? "created" : "updated")} successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, BaseResponse<Exception>.FailureResponse("Error creating project", [ex.Message]));
        }
    }
    
    [HttpGet("get-projects")]
    [Authorize]
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

    [HttpPost("get-projects-overview")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> GetAllProjectDetails(Guid projectId)
    {
        try
        {
            var projectDetails = await _projectService.GetProjectDetailsById(projectId);
            return Ok(BaseResponse<ProjectCardsDto>.SuccessResponse(projectDetails));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, BaseResponse<Exception>.FailureResponse("Error retrieving project", [ex.Message]));
        }
    }

    [HttpGet("get-project-details-for-edit")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> GetProjectDetailsForUpdate(Guid projectId, CancellationToken cancellation)
    {
        try
        {
            var projectDetails = await _projectService.GetProjectDetailsForUpdateAsync(projectId, cancellation);
            return Ok(BaseResponse<UpsertProjectDto>.SuccessResponse(projectDetails));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, BaseResponse<Exception>.FailureResponse("Error retrieving project details for update", [ex.Message]));
        }
    }

    [HttpDelete("delete-project/{projectId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellation)
    {
        try
        {
            await _projectService.DeleteProject(projectId, cancellation);
            return Ok(BaseResponse<string>.SuccessResponse("Project deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, BaseResponse<Exception>.FailureResponse("Error deleting project", [ex.Message]));
        }
    }

    [HttpGet("get-projects-user-to-invite")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> GetUserDetailsToInviteAsync(Guid? projectId, CancellationToken cancellation)
    {
        try
        {
            var userDetails = await _userService.GetUserDetailsToInviteAsync(projectId, cancellation);
            return Ok(BaseResponse<List<UserToInviteInProject>>.SuccessResponse(userDetails));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, BaseResponse<Exception>.FailureResponse("Error retrieving user details", [ex.Message]));
        }
    }
}