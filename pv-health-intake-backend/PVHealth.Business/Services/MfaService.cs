using System.Security.Cryptography;
using OtpNet;
using PVHealth.Common.DTOs;
using PVHealth.Data.Context;

namespace PVHealth.Business.Services;

public class MfaService: IMfaService
{
    private readonly ApplicationDbContext _context;
    public MfaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseDTO<MfaSetupResponse>> GenerateMfaSetupAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ResponseDTO<MfaSetupResponse>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var secret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
            var appName = "PV Health";
            var qrCodeUrl = $"otpauth://totp/{appName}:{user.Email}?secret={secret}&issuer={appName}";

            var backupCodes = GenerateBackupCodes();

            return new ResponseDTO<MfaSetupResponse>
            {
                Success = true,
                Data = new MfaSetupResponse
                {
                    Secret = secret,
                    QrCodeUrl = qrCodeUrl,
                    BackupCodes = backupCodes
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<MfaSetupResponse>
            {
                Success = false,
                Message = "Failed to generate MFA setup",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ResponseDTO<bool>> EnableMfaAsync(Guid userId, string code, string? secret = null)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            string secretToUse;
            List<string> backupCodes;

            if (!string.IsNullOrEmpty(secret))
            {
                // Use the provided secret from frontend
                secretToUse = secret;
                backupCodes = GenerateBackupCodes();
            }
            else
            {
                // Generate new setup (fallback)
                var setupResponse = await GenerateMfaSetupAsync(userId);
                if (!setupResponse.Success || setupResponse.Data == null)
                {
                    return new ResponseDTO<bool>
                    {
                        Success = false,
                        Message = "Failed to generate MFA setup"
                    };
                }
                secretToUse = setupResponse.Data.Secret;
                backupCodes = setupResponse.Data.BackupCodes;
            }

            var totp = new Totp(Base32Encoding.ToBytes(secretToUse));
            
            if (!totp.VerifyTotp(code, out _, new VerificationWindow(2, 2)))
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = "Invalid verification code"
                };
            }

            user.MfaEnabled = true;
            user.MfaSecret = secretToUse;
            user.BackupCodes = string.Join(",", backupCodes.Select(HashCode));

            await _context.SaveChangesAsync();

            return new ResponseDTO<bool>
            {
                Success = true,
                Data = true,
                Message = "MFA enabled successfully"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<bool>
            {
                Success = false,
                Message = "Failed to enable MFA",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ResponseDTO<bool>> DisableMfaAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            user.MfaEnabled = false;
            user.MfaSecret = null;
            user.BackupCodes = null;

            await _context.SaveChangesAsync();

            return new ResponseDTO<bool>
            {
                Success = true,
                Data = true,
                Message = "MFA disabled successfully"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<bool>
            {
                Success = false,
                Message = "Failed to disable MFA",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ResponseDTO<bool>> VerifyMfaCodeAsync(Guid userId, string code)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.MfaEnabled || string.IsNullOrEmpty(user.MfaSecret))
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = "MFA not enabled for this user"
                };
            }

            var totp = new Totp(Base32Encoding.ToBytes(user.MfaSecret));
            var isValid = totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));

            return new ResponseDTO<bool>
            {
                Success = isValid,
                Data = isValid,
                Message = isValid ? "Code verified" : "Invalid code"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<bool>
            {
                Success = false,
                Message = "Failed to verify code",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ResponseDTO<bool>> VerifyBackupCodeAsync(Guid userId, string code)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.MfaEnabled || string.IsNullOrEmpty(user.BackupCodes))
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = "No backup codes available"
                };
            }

            var backupCodes = user.BackupCodes.Split(',').ToList();
            var hashedCode = HashCode(code);

            if (backupCodes.Contains(hashedCode))
            {
                backupCodes.Remove(hashedCode);
                user.BackupCodes = string.Join(",", backupCodes);
                await _context.SaveChangesAsync();

                return new ResponseDTO<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Backup code verified"
                };
            }

            return new ResponseDTO<bool>
            {
                Success = false,
                Message = "Invalid backup code"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<bool>
            {
                Success = false,
                Message = "Failed to verify backup code",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ResponseDTO<MfaStatusResponse>> GetMfaStatusAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ResponseDTO<MfaStatusResponse>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            return new ResponseDTO<MfaStatusResponse>
            {
                Success = true,
                Data = new MfaStatusResponse
                {
                    MfaEnabled = user.MfaEnabled
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<MfaStatusResponse>
            {
                Success = false,
                Message = "Failed to get MFA status",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private List<string> GenerateBackupCodes()
    {
        var codes = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            codes.Add(GenerateRandomCode());
        }
        return codes;
    }

    private string GenerateRandomCode()
    {
        var bytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        var code = BitConverter.ToUInt32(bytes, 0) % 100000000;
        return code.ToString("D8");
    }

    private string HashCode(string code)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(code);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}