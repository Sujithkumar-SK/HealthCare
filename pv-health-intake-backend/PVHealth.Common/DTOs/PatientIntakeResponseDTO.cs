namespace PVHealth.Common.DTOs;

public class PatientIntakeResponseDTO
{
    public PatientDTO Patient { get; set; } = null!;
    public SurveyDTO Survey { get; set; } = null!;
}
