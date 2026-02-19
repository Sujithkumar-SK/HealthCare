using System.Text.Json;
using PVHealth.Common.DTOs;
using PVHealth.Data.Cache;

namespace PVHealth.Business.Services;

public class LocationService :ILocationService
{
    private readonly RedisCacheService _cache;
    private readonly HttpClient _httpclient;
    private const string ApiKey = "a862e4a98baff7f9c94f78a0549da13f7777815b5b967bf0754c9e769a4ad796";
    private const string BaseUrl = "https://api.countrystatecity.in/v1";
    public LocationService(RedisCacheService cache, IHttpClientFactory httpClientFactory)
    {
        _cache = cache;
        _httpclient = httpClientFactory.CreateClient();
        _httpclient.DefaultRequestHeaders.Add("X-CSCAPI-KEY",ApiKey);
    }
    public async Task<List<CountryDto>>GetCountriesAsync()
    {
        var cached = await _cache.GetAsync<List<CountryDto>>("location:countries");
        if (cached != null) return cached;
        var response = await _httpclient.GetStringAsync($"{BaseUrl}/countries");
        var countries = JsonSerializer.Deserialize<List<CountryDto>>(response) ?? new();
        await _cache.SetAsync("location:countries",countries,null);
        return countries;
    }
    public async Task<List<StateDto>>GetStatesByCountryAsync(string CountryCode)
    {
        var key = $"location:states:{CountryCode}";
        var cached = await _cache.GetAsync<List<StateDto>>(key);
        if (cached !=null) return cached;
        var response = await _httpclient.GetStringAsync($"{BaseUrl}/countries/{CountryCode}/states");
        var states = JsonSerializer.Deserialize<List<StateDto>>(response) ?? new();
        await _cache.SetAsync(key,states,null);
        return states;
    }
    public async Task<List<CityDto>>GetCitiesByStateAsync(string CountryCode, string StateCode)
    {
        var key = $"location:cities:{CountryCode}:{StateCode}";
        var cached = await _cache.GetAsync<List<CityDto>>(key);
        if (cached != null)return cached;
        var response = await _httpclient.GetStringAsync($"{BaseUrl}/countries/{CountryCode}/states/{StateCode}/cities");
        var cities = JsonSerializer.Deserialize<List<CityDto>>(response) ?? new();
        await _cache.SetAsync(key,cities,null);
        return cities;
    }
    public async Task PrefetchCountryDataAsync(string CountryCode)
    {
        var states = await GetStatesByCountryAsync(CountryCode);
        var cityTasks = states.Select(state => GetCitiesByStateAsync(CountryCode, state.Iso2));
        await Task.WhenAll(cityTasks);
    }

}