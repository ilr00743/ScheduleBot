using System.Net;
using System.Net.Http.Json;
using Core.Entities;

namespace Bot.ApiClients;

public class DepartmentApiClient
{
    private readonly HttpClient _httpClient;

    public DepartmentApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Department>?> GetDepartmentsAsync()
    {
        var response = await _httpClient.GetAsync("api/departments");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<Department>>();
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/departments/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Department>();
    }

    public async Task<Department?> GetDepartmentByNameAsync(string name)
    {
        var response = await _httpClient.GetAsync($"api/departments/by-name/{name}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Department>();
    }
}