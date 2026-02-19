namespace PVHealth.Common.DTOs;

public class MfaSetupResponse
{
    public string Secret {get;set;} = string.Empty;
    public string QrCodeUrl {get;set;} = string.Empty;
    public List<string> BackupCodes {get;set;}= new();
}

public class MfaVerifyRequest
{
    public string Code { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public class MfaEnableRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Secret { get; set; }
}

public class MfaStatusResponse
{
    public bool MfaEnabled { get; set; }
}