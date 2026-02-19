using PVHealth.Common.DTOs;

namespace PVHealth.Business.Services;

public interface IAuthService
{
    Task<AuthResponse>AuthenticateWithGoogleAsync(string idToken);
    Task<AuthResponse>AuthenticateWithGitHubAsync(string code);
    string GenerateJwtToken(Guid userId, string email, string name);
}