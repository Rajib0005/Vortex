using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vortex.Domain.Dto;
using Vortex.Infrastructure.Interfaces;

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

    [HttpGet("token")]
    public async Task<ActionResult> GetToken(Guid userId, string email, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _authService.GenerateTokenAsync(userId, email, cancellationToken);
            return Ok(BaseResponse<string>.SuccessResponse(token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }

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
            return StatusCode(500);
        }

    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Signup(AuthDto user)
    {
       var token =  await _authService.SingUpAsync(user);
        return Ok(BaseResponse<string>.SuccessResponse(token, "Token generated successfully"));
    }

    [HttpGet]
    [Authorize]
    [Route("me")]
    public async Task<IActionResult> GetUserDetails(Guid userId, CancellationToken cancellationToken)
    {
        var userDetails = await _authService.GetUserDetailsByIdAsync(userId, cancellationToken);
        return Ok(BaseResponse<UserDetailsDto>.SuccessResponse(userDetails));
    }
}