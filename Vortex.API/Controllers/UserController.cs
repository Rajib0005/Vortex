using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.API.shared.Attributes;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;

namespace Vortex.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Route("get-invite-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetInviteUserDetails(CancellationToken cancellationToken)
    {
        var inviteUserDetailsModel = await userService.GetInviteUserDetails(cancellationToken);
        return Ok(BaseResponse<InviteUserDetails>.SuccessResponse(inviteUserDetailsModel));
    }
}