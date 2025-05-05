using System.Net;
using System.Net.Http.Json;
using Core.Entities;

namespace Bot.ApiClients;

public class LessonChangesApiClient
{
    private readonly HttpClient _httpClient;
    
    public LessonChangesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<LessonChange>?> GetChanges(int? groupId = null, int? teacherId = null, DateOnly? date = null)
    {
        var queryParams = new List<string>();

        if (groupId.HasValue)
        {
            queryParams.Add($"groupId={groupId}");
        }

        if (teacherId.HasValue)
        {
            queryParams.Add($"teacherId={teacherId}");
        }

        if (date.HasValue)
        {
            queryParams.Add($"date={date}");
        }
        
        string query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;

        var response = await _httpClient.GetAsync($"api/changes{query}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<LessonChange>>();
    }

    public async Task<LessonChange?> GetChangesById(int id)
    {
        var response = await _httpClient.GetAsync($"api/changes/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<LessonChange>();
    }

    public async Task<bool> CreateLessonChanges(LessonChange change)
    {
        var response = await _httpClient.PostAsJsonAsync("api/changes", change);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateLessonChanges(int id, LessonChange change)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/changes/{id}", change);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteLessonChanges(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/changes/{id}");
        return response.IsSuccessStatusCode;
    }
}