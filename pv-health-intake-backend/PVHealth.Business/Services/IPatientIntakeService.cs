using PVHealth.Common.DTOs;

namespace PVHealth.Business.Services;

public interface IPatientIntakeService
{
    Task<ResponseDTO<PatientIntakeResponseDTO>> CreatePatientWithSurveyAsync(PatientDTO patientDto, SurveyDTO surveyDto);
}
