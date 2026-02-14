using PVHealth.Common.DTOs;

namespace PVHealth.Business.Services;

public interface ILocationService
{
    Task<List<CountryDto>>GetCountriesAsync();
    Task<List<StateDto>>GetStatesByCountryAsync(string CountryCode);
    Task<List<CityDto>>GetCitiesByStateAsync(string CountryCode, string StateCode);
    Task PrefetchCountryDataAsync(string CountryCode);
}