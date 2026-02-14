using PVHealth.Common.DTOs;

namespace PVHealth.Business.Services;

public interface ISurveyService
{
    Task<ResponseDTO<SurveyDTO>> CreateSurveyAsync(SurveyDTO surveyDto);
    Task<ResponseDTO<SurveyDTO>> GetSurveyByIdAsync(Guid id);
    Task<ResponseDTO<SurveyDTO>> GetSurveyByPatientIdAsync(Guid patientId);
    Task<ResponseDTO<SurveyDTO>> UpdateSurveyAsync(Guid id, SurveyDTO surveyDto);
    Task<ResponseDTO<bool>> DeleteSurveyAsync(Guid id);
}
