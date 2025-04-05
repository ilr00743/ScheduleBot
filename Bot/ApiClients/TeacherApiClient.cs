using System.Net;
using System.Net.Http.Json;
using Core.Entities;

namespace Bot.ApiClients;

public class TeacherApiClient
{
    private readonly HttpClient _httpClient;

    public TeacherApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Teacher>?> GetTeachersAsync()
    {
        var response = await _httpClient.GetAsync("api/teachers");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<Teacher>>();
    }

    public async Task<Teacher?> GetTeacherByIdAsync(int teacherId)
    {
        var response = await _httpClient.GetAsync($"api/teachers/{teacherId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Teacher>();
    }

    public async Task<List<Teacher>?> GetTeachersByDepartment(int departmentId)
    {
        var response = await _httpClient.GetAsync($"api/teachers/by-department/{departmentId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<List<Teacher>>();
    }

    public async Task<Teacher?> GetTeacherByFullName(string fullName)
    {
        var response = await _httpClient.GetAsync($"api/teachers/by-name?fullName={fullName}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Teacher>();
    }
}