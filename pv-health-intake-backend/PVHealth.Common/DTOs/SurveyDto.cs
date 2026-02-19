namespace PVHealth.Common.DTOs;

public class SurveyDTO
{
    public Guid? Id { get; set; }
    public Guid PatientId { get; set; }
    public object SurveyData { get; set; } = new { };
    public DateTime? CreatedAt { get; set; }
}
