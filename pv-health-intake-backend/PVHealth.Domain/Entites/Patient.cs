namespace PVHealth.Domain.Entities;
public class Patient: BaseEntity
{
    public string FullName {get;set;}= string.Empty;
    public DateTime DateOfBirth {get;set;}
    public string Email {get;set;} = string.Empty;
    public string Phone {get;set;}=string.Empty;
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;

    public Survey? Survey {get;set;}

}