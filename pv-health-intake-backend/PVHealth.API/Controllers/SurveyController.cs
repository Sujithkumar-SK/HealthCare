using Microsoft.AspNetCore.Mvc;
using PVHealth.Business.Services;
using PVHealth.Common.DTOs;

namespace PVHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveysController : ControllerBase
{
    private readonly ISurveyService _surveyService;

    public SurveysController(ISurveyService surveyService)
    {
        _surveyService = surveyService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSurvey([FromBody] SurveyDTO surveyDto)
    {
        var result = await _surveyService.CreateSurveyAsync(surveyDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSurveyById(Guid id)
    {
        var result = await _surveyService.GetSurveyByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetSurveyByPatientId(Guid patientId)
    {
        var result = await _surveyService.GetSurveyByPatientIdAsync(patientId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSurvey(Guid id, [FromBody] SurveyDTO surveyDto)
    {
        var result = await _surveyService.UpdateSurveyAsync(id, surveyDto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSurvey(Guid id)
    {
        var result = await _surveyService.DeleteSurveyAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
