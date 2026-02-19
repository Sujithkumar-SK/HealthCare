using Microsoft.AspNetCore.Mvc;
using PVHealth.Business.Services;
using PVHealth.Common.DTOs;

namespace PVHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientIntakeController : ControllerBase
{
    private readonly IPatientIntakeService _intakeService;

    public PatientIntakeController(IPatientIntakeService intakeService)
    {
        _intakeService = intakeService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatientWithSurvey([FromBody] PatientIntakeRequest request)
    {
        var result = await _intakeService.CreatePatientWithSurveyAsync(request.Patient, request.Survey);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public class PatientIntakeRequest
{
    public PatientDTO Patient { get; set; } = null!;
    public SurveyDTO Survey { get; set; } = null!;
}
