using Microsoft.AspNetCore.Mvc;
using PVHealth.Business.Services;
using PVHealth.Common.DTOs;

namespace PVHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request)
    {
        try
        {
            var result = await _authService.AuthenticateWithGoogleAsync(request.IdToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("github")]
    public async Task<IActionResult> GitHubAuth([FromBody] GitHubAuthRequest request)
    {
        try
        {
            var result = await _authService.AuthenticateWithGitHubAsync(request.Code);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}

public class GoogleAuthRequest
{
    public string IdToken { get; set; } = string.Empty;
}

public class GitHubAuthRequest
{
    public string Code { get; set; } = string.Empty;
}
