using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PVHealth.Common.DTOs;
using PVHealth.Data.Cache;
using PVHealth.Data.Context;
using PVHealth.Data.Repositories;
using PVHealth.Domain.Entities;
using PVHealth.Domain.Interfaces;

namespace PVHealth.Business.Services;
public class SurveyService : ISurveyService
{
    private readonly IRepository<Survey> _repo;
    private readonly SurveyRepository _surveyrepo;
    private readonly RedisCacheService _cache;
    private readonly ApplicationDbContext _context;

    public SurveyService(IRepository<Survey> repo, SurveyRepository surveyrepo, RedisCacheService cache, ApplicationDbContext context)
    {
        _repo = repo;
        _surveyrepo = surveyrepo;
        _cache = cache;
        _context = context;
    }
    public async Task<ResponseDTO<SurveyDTO>> CreateSurveyAsync(SurveyDTO surveyDto)
    {
        using var transcation = await _context.Database.BeginTransactionAsync();
        try
        {
            var patientExists = await _context.Patients.AnyAsync(p=>p.Id == surveyDto.PatientId);
            if(!patientExists)
            {
                return new ResponseDTO<SurveyDTO>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }
            var survey = new Survey
            {
                PatientId = surveyDto.PatientId,
                SurveyData = JsonSerializer.Serialize(surveyDto.SurveyData),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _repo.AddAsync(survey);
            if (created==null)
            {
                return new ResponseDTO<SurveyDTO>
                {
                    Success = false,
                    Message = "Failed to update in db"
                };
            }
            await transcation.CommitAsync();
            await _cache.SetAsync($"survey:{created.Id}", created);

            surveyDto.Id = created.Id;
            return new ResponseDTO<SurveyDTO>
            {
                Success = true,
                Message = "Survey created successfully",
                Data = surveyDto
            };
        }
        catch (Exception ex)
        {
            await transcation.RollbackAsync();
            return new ResponseDTO<SurveyDTO>
            {
                Success = false,
                Message = "Failed to create survey",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDTO<SurveyDTO>> GetSurveyByIdAsync(Guid id)
    {
        try
        {
            var cached = await _cache.GetAsync<Survey>($"survey:{id}");
            if (cached != null)
            {
                return new ResponseDTO<SurveyDTO>
                {
                    Success = true,
                    Data = MapToDto(cached)
                };
            }

            var survey = await _repo.GetByIdAsync(id);
            if (survey == null)
            {
                return new ResponseDTO<SurveyDTO>
                {
                    Success = false,
                    Message = "Survey not found"
                };
            }

            await _cache.SetAsync($"survey:{id}", survey);
            return new ResponseDTO<SurveyDTO>
            {
                Success = true,
                Data = MapToDto(survey)
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<SurveyDTO>
            {
                Success = false,
                Message = "Failed to retrieve survey",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDTO<SurveyDTO>> GetSurveyByPatientIdAsync(Guid patientId)
    {
        try
        {
            var survey = await _surveyrepo.GetByPatientIdAsync(patientId);
            if (survey == null)
            {
                return new ResponseDTO<SurveyDTO>
                {
                    Success = false,
                    Message = "Survey not found for this patient"
                };
            }

            return new ResponseDTO<SurveyDTO>
            {
                Success = true,
                Data = MapToDto(survey)
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<SurveyDTO>
            {
                Success = false,
                Message = "Failed to retrieve survey",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDTO<SurveyDTO>> UpdateSurveyAsync(Guid id, SurveyDTO surveyDto)
    {
        try
        {
            var survey = await _repo.GetByIdAsync(id);
            if (survey == null)
            {
                return new ResponseDTO<SurveyDTO>
                {
                    Success = false,
                    Message = "Survey not found"
                };
            }

            survey.SurveyData = JsonSerializer.Serialize(surveyDto.SurveyData);
            await _repo.UpdateAsync(survey);
            await _cache.DeleteAsync($"survey:{id}");

            return new ResponseDTO<SurveyDTO>
            {
                Success = true,
                Message = "Survey updated successfully",
                Data = MapToDto(survey)
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<SurveyDTO>
            {
                Success = false,
                Message = "Failed to update survey",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDTO<bool>> DeleteSurveyAsync(Guid id)
    {
        try
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted)
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = "Survey not found"
                };
            }

            await _cache.DeleteAsync($"survey:{id}");
            return new ResponseDTO<bool>
            {
                Success = true,
                Message = "Survey deleted successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<bool>
            {
                Success = false,
                Message = "Failed to delete survey",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private SurveyDTO MapToDto(Survey survey)
    {
        return new SurveyDTO
        {
            Id = survey.Id,
            PatientId = survey.PatientId,
            SurveyData = JsonSerializer.Deserialize<object>(survey.SurveyData) ?? new { }
        };
    }
}