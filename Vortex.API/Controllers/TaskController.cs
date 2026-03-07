using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.Application.Dtos;
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
        var result = await _taskService.CreateTaskAsync(dto, ct);
        return Ok(BaseResponse<TaskDto>.SuccessResponse(result, "Task created successfully"));
    }

    [HttpPut("{taskId}/update-task")]
    [Authorize]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpsertTaskDto dto, CancellationToken ct)
    {
        var result = await _taskService.UpdateTaskAsync(taskId, dto, ct);
        return Ok(BaseResponse<TaskDto>.SuccessResponse(result, "Task updated successfully"));
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

    [HttpGet("{projectId}/tasks")]
    [Authorize]
    public async Task<IActionResult> GetTasksByProject(Guid projectId, CancellationToken ct)
    {
        var result = await _taskService.GetTasksByProjectAsync(projectId, ct);
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
