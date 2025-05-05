using System.Net;
using System.Net.Http.Json;
using Core.Entities;

namespace Bot.ApiClients;

public class LessonApiClient
{
    private readonly HttpClient _httpClient;

    public LessonApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Lesson>?> GetLessons(int? groupId = null, int? teacherId = null, int? dayId = null)
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

        if (dayId.HasValue)
        {
            queryParams.Add($"dayId={dayId}");
        }
        
        string query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;

        var response = await _httpClient.GetAsync($"api/lessons{query}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<Lesson>>();
    }

    public async Task<Lesson?> GetLessonById(int lessonId)
    {
        var response = await _httpClient.GetAsync($"api/lessons/{lessonId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Lesson>();
    }

    public async Task<bool> CreateLesson(Lesson lesson)
    {
        var response = await _httpClient.PostAsJsonAsync("api/lessons", lesson);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateLesson(int lessonId, Lesson lesson)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/lessons/{lessonId}", lesson);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteLesson(int lessonId)
    {
        var response = await _httpClient.DeleteAsync($"api/lessons/{lessonId}");
        return response.IsSuccessStatusCode;
    }
}