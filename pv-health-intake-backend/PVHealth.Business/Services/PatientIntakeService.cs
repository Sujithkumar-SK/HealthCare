using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PVHealth.Common.DTOs;
using PVHealth.Data.Cache;
using PVHealth.Data.Context;
using PVHealth.Domain.Entities;

namespace PVHealth.Business.Services;

public class PatientIntakeService : IPatientIntakeService
{
    private readonly ApplicationDbContext _context;
    private readonly RedisCacheService _cache;

    public PatientIntakeService(ApplicationDbContext context, RedisCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<ResponseDTO<PatientIntakeResponseDTO>> CreatePatientWithSurveyAsync(
        PatientDTO patientDto, 
        SurveyDTO surveyDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var patient = new Patient
            {
                FullName = patientDto.FullName,
                DateOfBirth = patientDto.DateOfBirth,
                Email = patientDto.Email,
                Phone = patientDto.Phone,
                Country = patientDto.Country,
                State = patientDto.State,
                City = patientDto.City,
                AppointmentDate = patientDto.AppointmentDate,
                ReasonForVisit = patientDto.ReasonForVisit,
                UserId = patientDto.UserId ?? Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var survey = new Survey
            {
                PatientId = patient.Id,
                SurveyData = JsonSerializer.Serialize(surveyDto.SurveyData),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            await _cache.SetAsync($"patient:{patient.Id}", patient);
            await _cache.SetAsync($"survey:{survey.Id}", survey);

            patientDto.Id = patient.Id;
            surveyDto.Id = survey.Id;
            surveyDto.PatientId = patient.Id;

            return new ResponseDTO<PatientIntakeResponseDTO>
            {
                Success = true,
                Message = "Patient and survey created successfully",
                Data = new PatientIntakeResponseDTO
                {
                    Patient = patientDto,
                    Survey = surveyDto
                }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ResponseDTO<PatientIntakeResponseDTO>
            {
                Success = false,
                Message = "Failed to create patient and survey. Transaction rolled back.",
                Errors = new List<string> { ex.Message, ex.InnerException?.Message ?? "" }
            };
        }
    }
}
