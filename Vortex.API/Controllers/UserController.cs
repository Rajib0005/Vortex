using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;
using Vortex.Infrastructure.Interfaces;
using ProjectRoleDto = Vortex.Application.Dtos.ProjectRoleDto;

namespace Vortex.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }
    
    [HttpGet]
    [Authorize]
    [Route("me")]
    public async Task<IActionResult> GetUserDetails(CancellationToken cancellationToken)
    {
        try
        {
            var userDetails = await _userService.GetUserDetailsByIdAsync(cancellationToken);
            return Ok(BaseResponse<UserDetailsDto>.SuccessResponse(userDetails));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Unauthorized(BaseResponse<Exception>.FailureResponse("Unauthorized", [ex.Message]));
        }
    }
    
    [HttpGet]
    [Route("get-invite-users")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> GetInviteUserDetails(CancellationToken cancellationToken)
    {
        var inviteUserDetailsModel = await _userService.GetInviteUserDetails(cancellationToken);
        return Ok(BaseResponse<ProjectRoleDto>.SuccessResponse(inviteUserDetailsModel));
    }
    
    [HttpPost]
    [Route("invite-users")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> InviteUserDetails([FromBody] List<InviteUserDto> inviteUserDto, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.InviteUserAsync(inviteUserDto, cancellationToken);
            return Ok(BaseResponse<string>.SuccessResponse("Invite users successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Invite users failed: {ex.Message}");
            return StatusCode(500, BaseResponse<string>.FailureResponse("An error occured while inviting users", [ex.Message]));
        }
    }
}