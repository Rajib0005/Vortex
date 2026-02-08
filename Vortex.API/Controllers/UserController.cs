using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Dto;
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
    [Route("users")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> GetAllUserDetails(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsers(cancellationToken);
        return Ok(BaseResponse<IList<UserDetailsDto>>.SuccessResponse(data: users));
    }
}