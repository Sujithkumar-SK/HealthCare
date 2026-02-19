using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PVHealth.Common.DTOs;
using PVHealth.Data.Cache;
using PVHealth.Data.Context;
using PVHealth.Domain.Entities;
using PVHealth.Domain.Interfaces;

namespace PVHealth.Business.Services;
public class PatientService : IPatientService
{
    private readonly IRepository<Patient> _repo;
    private readonly RedisCacheService _cache;
    private readonly ApplicationDbContext _context;
    public PatientService(IRepository<Patient> repo, RedisCacheService cache, ApplicationDbContext context)
    {
        _repo = repo;
        _cache = cache;
        _context = context;
    }
    public async Task<ResponseDTO<PatientDTO>> CreatePatientAsync(PatientDTO patientDto)
    {
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
                UserId = patientDto.UserId ?? Guid.Empty
            };
            var created = await _repo.AddAsync(patient);
            await _cache.SetAsync($"patient:{created.Id}",created);
            patientDto.Id = created.Id;
            return new ResponseDTO<PatientDTO>
            {
              Success = true,
              Message = "Patient created successfully",
              Data = patientDto  
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<PatientDTO>
            {
                Success = false,
                Message = "Failed to create patient",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ResponseDTO<PatientDTO>> GetPatientByIdAsync(Guid id)
    {
        try
        {
            var cached = await _cache.GetAsync<Patient>($"patient:{id}");
            if(cached != null)
            {
                return new ResponseDTO<PatientDTO>
                {
                    Success = true,
                    Data = MapToDto(cached)
                };
            }
            var patient = await _repo.GetByIdAsync(id);
            if( patient == null)
            {
                return new ResponseDTO<PatientDTO>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }
            await _cache.SetAsync($"patient:{id}",patient);
            return new ResponseDTO<PatientDTO>
            {
                Success = true,
                Data = MapToDto(patient)
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<PatientDTO>
            {
                Success = false,
                Message = "Failed to retrieve patient",
                Errors = new List<string> {ex.Message}
            };
        }
    }
    public async Task<ResponseDTO<IEnumerable<PatientDTO>>> GetAllPatientsAsync()
    {
        try
        {
            var patients = await _repo.GetAllAsync();
            var dtos = patients.Select(MapToDto);

            return new ResponseDTO<IEnumerable<PatientDTO>>
            {
                Success = true,
                Data = dtos
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<IEnumerable<PatientDTO>>
            {
                Success = false,
                Message = "Failed to retrieve patients",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ResponseDTO<PatientDTO>> UpdatePatientAsync(Guid id, PatientDTO patientDto)
    {
        try
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient == null)
            {
                return new ResponseDTO<PatientDTO>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }

            patient.FullName = patientDto.FullName;
            patient.DateOfBirth = patientDto.DateOfBirth;
            patient.Email = patientDto.Email;
            patient.Phone = patientDto.Phone;
            patient.Country = patientDto.Country;
            patient.State = patientDto.State;
            patient.City = patientDto.City;
            patient.AppointmentDate = patientDto.AppointmentDate;
            patient.ReasonForVisit = patientDto.ReasonForVisit;

            await _repo.UpdateAsync(patient);
            await _cache.DeleteAsync($"patient:{id}");

            return new ResponseDTO<PatientDTO>
            {
                Success = true,
                Message = "Patient updated successfully",
                Data = MapToDto(patient)
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<PatientDTO>
            {
                Success = false,
                Message = "Failed to update patient",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ResponseDTO<bool>> DeletePatientAsync(Guid id)
    {
        try
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted)
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }

            await _cache.DeleteAsync($"patient:{id}");
            return new ResponseDTO<bool>
            {
                Success = true,
                Message = "Patient deleted successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<bool>
            {
                Success = false,
                Message = "Failed to delete patient",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    private PatientDTO MapToDto(Patient patient)
    {
        return new PatientDTO
        {
            Id = patient.Id,
            FullName = patient.FullName,
            DateOfBirth = patient.DateOfBirth,
            Email = patient.Email,
            Phone = patient.Phone,
            Country = patient.Country,
            State = patient.State,
            City = patient.City,
            AppointmentDate = patient.AppointmentDate,
            ReasonForVisit = patient.ReasonForVisit,
            UserId = patient.UserId
        };
    }

    public async Task<ResponseDTO<IEnumerable<PatientIntakeResponseDTO>>> GetSurveysByUserIdAsync(Guid userId)
    {
        try
        {
            var patients = await _context.Patients
                .Include(p => p.Survey)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var result = patients.Select(p => new PatientIntakeResponseDTO
            {
                Patient = MapToDto(p),
                Survey = p.Survey != null ? new SurveyDTO
                {
                    Id = p.Survey.Id,
                    PatientId = p.Id,
                    SurveyData = JsonSerializer.Deserialize<object>(p.Survey.SurveyData),
                    CreatedAt = p.Survey.CreatedAt
                } : null!
            });

            return new ResponseDTO<IEnumerable<PatientIntakeResponseDTO>>
            {
                Success = true,
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<IEnumerable<PatientIntakeResponseDTO>>
            {
                Success = false,
                Message = "Failed to retrieve surveys",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDTO<IEnumerable<PatientDTO>>> GetPatientsByUserIdAsync(Guid userId)
    {
        try
        {
            var patients = await _context.Patients
                .Include(p => p.Survey)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var result = patients.Select(p => {
                var dto = MapToDto(p);
                dto.HasSurvey = p.Survey != null;
                return dto;
            }).ToList();

            return new ResponseDTO<IEnumerable<PatientDTO>>
            {
                Success = true,
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<IEnumerable<PatientDTO>>
            {
                Success = false,
                Message = "Failed to retrieve patients",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}