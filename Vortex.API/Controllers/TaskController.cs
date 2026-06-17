using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.Application.Dtos;
using Vortex.Application.Dtos.Filtering;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;

namespace Vortex.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    [HttpPost("/create-task")]
    [Authorize]
    public async Task<IActionResult> CreateTask([FromBody] UpsertTaskDto dto, CancellationToken ct)
    {
        await _taskService.CreateTaskAsync(dto, ct);
        return Ok(BaseResponse<string>.SuccessResponse("Task created successfully"));
    }
    /// <summary>
    /// Multi-filter endpoint for tasks within a project.
    /// Query string example: ?Statuses=0&amp;Statuses=2&amp;AssigneeIds=guid&amp;Page=1&amp;PageSize=20
    /// </summary>
    [HttpGet("{projectId}/tasks")]
    [Authorize(Roles = "Admin, Manager, Member")]
    public async Task<IActionResult> GetFilteredTasks(
        Guid projectId,
        [FromQuery] TaskFilterQuery filter,
        CancellationToken ct)
    {
        var result = await _taskService.GetFilteredTasksAsync(projectId, filter, ct);
        return Ok(BaseResponse<PagedResult<TaskDto>>.SuccessResponse(result, "Tasks fetched successfully"));
    }

    [HttpPut("{taskId}/update-task")]
    [Authorize]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpsertTaskDto dto, CancellationToken ct)
    {
        await _taskService.UpdateTaskAsync(taskId, dto, ct);
        return Ok(BaseResponse<string>.SuccessResponse("Task updated successfully"));
    }

    [HttpDelete("{taskId}/delete-task")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> DeleteTask(Guid taskId, CancellationToken ct)
    {
        await _taskService.DeleteTaskAsync(taskId, ct);
        return Ok(BaseResponse<string>.SuccessResponse("Task deleted successfully"));
    }

    [HttpGet("{taskId}/task-details")]
    [Authorize]
    public async Task<IActionResult> GetTask(Guid taskId, CancellationToken ct)
    {
        var result = await _taskService.GetTaskAsync(taskId, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPatch("{taskId}/assign/{assigneeId}")]
    [Authorize]
    public async Task<IActionResult> AssignTask(Guid taskId, Guid assigneeId, CancellationToken ct)
    {
        await _taskService.AssignTaskAsync(taskId, assigneeId, ct);
        return Ok(BaseResponse<string>.SuccessResponse("Task assigned successfully"));
    }

}
