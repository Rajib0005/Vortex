using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;
using ProjectRoleDto = Vortex.Application.Dtos.ProjectRoleDto;

namespace Vortex.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService, ILogger _logger) : ControllerBase
{
    [HttpGet]
    [Route("get-invite-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetInviteUserDetails(CancellationToken cancellationToken)
    {
        var inviteUserDetailsModel = await userService.GetInviteUserDetails(cancellationToken);
        return Ok(BaseResponse<ProjectRoleDto>.SuccessResponse(inviteUserDetailsModel));
    }
    
    [HttpPost]
    [Route("invite-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InviteUserDetails([FromBody] List<InviteUserDto> inviteUserDto, CancellationToken cancellationToken)
    {
        try
        {
            await userService.InviteUserAsync(inviteUserDto, cancellationToken);
            return Ok(BaseResponse<string>.SuccessResponse("Invite users successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Invite users failed: {ex.Message}");
            return StatusCode(500, BaseResponse<string>.FailureResponse("An error occured while inviting users", [ex.Message]));
        }
    }
}