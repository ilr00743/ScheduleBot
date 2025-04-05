using System.Net;
using System.Net.Http.Json;
using Core.DTO;
using Core.Entities;

namespace Bot.ApiClients;

public class CourseApiClient
{
    private readonly HttpClient _httpClient;

    public CourseApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Course>?> GetCoursesAsync()
    {
        var response = await _httpClient.GetAsync("api/courses");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<Course>>();
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/courses/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Course>();
    }
    
    public async Task<Course?> GetCourseByNumberAsync(int number)
    {
        var response = await _httpClient.GetAsync($"api/courses/by-number/{number}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Course>();
    }

    public async Task<bool> CreateCourseAsync(Course course)
    {
        var response = await _httpClient.PostAsJsonAsync("api/courses", course);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCourseAsync(int number, List<Group> groups)
    {
        var request = new UpdateCourseRequest {Number = number, Groups = groups};
        var response = await _httpClient.PutAsJsonAsync("api/courses", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/courses/{id}");
        return response.IsSuccessStatusCode;
    }
}