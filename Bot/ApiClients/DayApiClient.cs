using System.Net;
using System.Net.Http.Json;
using Core.Entities;

namespace Bot.ApiClients;

public class DayApiClient
{
    private readonly HttpClient _httpClient;

    public DayApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<WeekDay>?> GetDays()
    {
        var response = await _httpClient.GetAsync("api/days");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<WeekDay>>();
    }

    public async Task<WeekDay?> GetDayById(int dayId)
    {
        var response = await _httpClient.GetAsync($"api/days/{dayId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<WeekDay>();
    }

    public async Task<WeekDay?> GetDayByName(string dayName)
    {
        var response = await _httpClient.GetAsync($"api/days/by-name?dayName={dayName}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<WeekDay>();
    }
}