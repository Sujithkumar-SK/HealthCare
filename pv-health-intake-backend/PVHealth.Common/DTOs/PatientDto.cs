namespace PVHealth.Common.DTOs;
public class PatientDTO
{
    public Guid? Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public bool HasSurvey { get; set; }
}