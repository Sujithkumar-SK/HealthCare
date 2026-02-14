using PVHealth.Common.DTOs;

namespace PVHealth.Business.Services;
public interface IPatientService
{
    Task<ResponseDTO<PatientDTO>> CreatePatientAsync(PatientDTO patientDto);
    Task<ResponseDTO<PatientDTO>> GetPatientByIdAsync(Guid id);
    Task<ResponseDTO<IEnumerable<PatientDTO>>> GetAllPatientsAsync();
    Task<ResponseDTO<PatientDTO>> UpdatePatientAsync(Guid id, PatientDTO patientDto);
    Task<ResponseDTO<bool>> DeletePatientAsync(Guid id);
}