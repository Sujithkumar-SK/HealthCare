using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVHealth.Business.Services;
using PVHealth.Common.DTOs;
using System.Security.Claims;

namespace PVHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MfaController : ControllerBase
{
    private readonly IMfaService _mfaService;

    public MfaController(IMfaService mfaService)
    {
        _mfaService = mfaService;
    }

    [HttpGet("setup/{userId}")]
    public async Task<IActionResult> GetMfaSetup(Guid userId)
    {
        var result = await _mfaService.GenerateMfaSetupAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("enable/{userId}")]
    public async Task<IActionResult> EnableMfa(Guid userId, [FromBody] MfaEnableRequest request)
    {
        var result = await _mfaService.EnableMfaAsync(userId, request.Code, request.Secret);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("disable/{userId}")]
    public async Task<IActionResult> DisableMfa(Guid userId)
    {
        var result = await _mfaService.DisableMfaAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyMfaCode([FromBody] MfaVerifyRequest request)
    {
        var result = await _mfaService.VerifyMfaCodeAsync(request.UserId, request.Code);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("verify-backup")]
    public async Task<IActionResult> VerifyBackupCode([FromBody] MfaVerifyRequest request)
    {
        var result = await _mfaService.VerifyBackupCodeAsync(request.UserId, request.Code);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("status/{userId}")]
    public async Task<IActionResult> GetMfaStatus(Guid userId)
    {
        var result = await _mfaService.GetMfaStatusAsync(userId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
