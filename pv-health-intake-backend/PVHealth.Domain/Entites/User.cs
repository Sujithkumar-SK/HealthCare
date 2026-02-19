namespace PVHealth.Domain.Entities;

public class User : BaseEntity
{
    public string Email {get;set;} = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OAuthProvider { get; set; } = string.Empty;
    public string OAuthId { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }

    public bool MfaEnabled {get;set;} = false;
    public string? MfaSecret {get;set;}
    public string? BackupCodes {get;set;}
    
    public ICollection<Patient> Patients {get;set;} = new List<Patient>();

}