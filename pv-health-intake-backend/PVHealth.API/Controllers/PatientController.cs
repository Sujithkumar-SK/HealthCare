using Microsoft.AspNetCore.Mvc;
using PVHealth.Business.Services;
using PVHealth.Common.DTOs;

namespace PVHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] PatientDTO patientDto)
    {
        var result = await _patientService.CreatePatientAsync(patientDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(Guid id)
    {
        var result = await _patientService.GetPatientByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var result = await _patientService.GetAllPatientsAsync();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] PatientDTO patientDto)
    {
        var result = await _patientService.UpdatePatientAsync(id, patientDto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        var result = await _patientService.DeletePatientAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
