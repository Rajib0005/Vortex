using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.Domain.Dto;
using Vortex.Application.Interfaces;
using Vortex.Application.Dtos;

namespace Vortex.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login(AuthDto authModel, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _authService.Login(authModel, cancellationToken);
            return Ok(BaseResponse<string>.SuccessResponse(token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Unauthorized(BaseResponse<Exception>.FailureResponse("Unauthorized", [ex.Message]));
        }

    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Signup(AuthDto user)
    {
        try
        {
            var token =  await _authService.SingUpAsync(user);
            return Ok(BaseResponse<string>.SuccessResponse(token, "Token generated successfully"));
        } catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, BaseResponse<Exception>.FailureResponse("Unauthorized", [ex.Message]));
        }
    }
    
    [HttpPost]
    [Route("invite")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> InviteUserDetails([FromBody] List<InviteUserDto> inviteUserDto, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.InviteUserAsync(inviteUserDto, cancellationToken);
            return Ok(BaseResponse<string>.SuccessResponse("Invite users successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Invite users failed: {ex.Message}");
            return StatusCode(500, BaseResponse<string>.FailureResponse("An error occured while inviting users", [ex.Message]));
        }
    }

    [HttpPost("set-password")]
    [AllowAnonymous]
    public async Task<ActionResult> SetPassword([FromBody] SetPasswordDto setPasswordDto)
    {
        try
        {
            await _authService.SetPassword(setPasswordDto);
            return Ok(BaseResponse<string>.SuccessResponse("Password set successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(BaseResponse<string>.FailureResponse("Failed to set password.", [ex.Message]));
        }
    }
}