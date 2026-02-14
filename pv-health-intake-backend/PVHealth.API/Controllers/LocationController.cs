using Microsoft.AspNetCore.Mvc;
using PVHealth.Business.Services;

namespace PVHealth.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _serv;
    public LocationController(ILocationService ser)
    {
        _serv = ser;
    }
    [HttpGet("countries")]
    public async Task<IActionResult>GetCountries()
    {
        var countries = await _serv.GetCountriesAsync();
        return Ok(countries);
    }
    [HttpGet("countries/{CountryCode}/states")]
    public async Task<IActionResult>GetStates(string CountryCode)
    {
        var states = await _serv.GetStatesByCountryAsync(CountryCode);
        return Ok(states);
    }
    [HttpGet("countries/{CountryCode}/states/{stateCode}/cities")]
    public async Task<IActionResult>GetCities(string CountryCode, string stateCode)
    {
        var cities = await _serv.GetCitiesByStateAsync(CountryCode,stateCode);
        return Ok(cities);
    }
    [HttpPost("countries/{CountryCode}/prefetch")]
    public async Task<IActionResult>PrefetchCountryData(string CountryCode)
    {
       await _serv.PrefetchCountryDataAsync(CountryCode);
       return Ok(new {message = "Data prefeteched successfully"}); 
    }
}