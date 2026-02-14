namespace PVHealth.Domain.Entities;
public class Survey:BaseEntity
{
    public Guid PatientId {get;set;}
    public string SurveyData {get;set;} = string.Empty;

    public Patient Patient{get;set;}= null!;
}