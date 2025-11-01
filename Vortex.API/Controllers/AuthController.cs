using System.Net;
using Microsoft.AspNetCore.Mvc;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Infrastructure.Interfaces;

namespace Vortex.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly ITokenService _tokenService;

    public AuthController(ILogger<AuthController> logger, ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
    }

    [HttpPost("token")]
    public ActionResult GetToken(AuthDto user)
    {
        try
        {
            var token = _tokenService.GenerateTokenAsync(user);
            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }

    }

    [HttpPost]
    [Route("register")]
    public ActionResult Signup(AuthDto user)
    {
        // TODO: To be implemented
        return Ok(user);
    }
}