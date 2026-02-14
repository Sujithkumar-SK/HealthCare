using System.Text.Json.Serialization;

namespace PVHealth.Common.DTOs;

public class CountryDto
{
    [JsonPropertyName("iso2")]
    public string Iso2 {get;set;} = string.Empty;
    [JsonPropertyName("name")]
    public string Name {get;set;} = string.Empty;
}
public class StateDto
{
    [JsonPropertyName("iso2")]
    public string Iso2 {get;set;} = string.Empty;
    [JsonPropertyName("name")]
    public string Name {get;set;} = string.Empty;
    [JsonPropertyName("country_code")]
    public string CountryCode {get;set;} = string.Empty;
}
public class CityDto
{
    [JsonPropertyName("name")]
    public string Name {get;set;} = string.Empty;
    [JsonPropertyName("state_code")]
    public string StateCode {get;set;} = string.Empty;
    [JsonPropertyName("country_code")]
    public string CountryCode {get;set;} = string.Empty;
}