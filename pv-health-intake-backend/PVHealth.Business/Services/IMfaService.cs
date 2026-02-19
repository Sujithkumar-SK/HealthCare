using PVHealth.Common.DTOs;

namespace PVHealth.Business.Services;

public interface IMfaService
{
    Task<ResponseDTO<MfaSetupResponse>> GenerateMfaSetupAsync(Guid userId);
    Task<ResponseDTO<bool>> EnableMfaAsync(Guid userId, string code, string? secret = null);
    Task<ResponseDTO<bool>> DisableMfaAsync(Guid userId);
    Task<ResponseDTO<bool>> VerifyMfaCodeAsync(Guid userId, string code);
    Task<ResponseDTO<bool>> VerifyBackupCodeAsync(Guid userId, string code);
    Task<ResponseDTO<MfaStatusResponse>> GetMfaStatusAsync(Guid userId);
}
