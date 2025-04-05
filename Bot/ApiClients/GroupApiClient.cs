using System.Net;
using System.Net.Http.Json;
using Core.Entities;

namespace PIBScheduleBot.ApiClients;

public class GroupApiClient
{
    private readonly HttpClient _httpClient;

    public GroupApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Group>?> GetGroups()
    {
        var response = await _httpClient.GetAsync("api/groups");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<Group>>();
    }

    public async Task<List<Group>?> GetGroupsByCourse(int courseId)
    {
        var response = await _httpClient.GetAsync($"api/groups?courseId={courseId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<List<Group>>();
    }
    
    public async Task<Group?> GetGroupById(int id)
    {
        var response = await _httpClient.GetAsync($"api/groups/{id}");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Group>();
    }
    
    public async Task<Group?> GetGroupByNumber(int number)
    {
        var response = await _httpClient.GetAsync($"api/groups/by-number/{number}");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Group>();
    }

    public async Task<bool> CreateGroup(Group group)
    {
        var response = await _httpClient.PostAsJsonAsync("api/groups", group);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateGroup(int id, Group group)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/groups/{id}", group);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteGroup(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/groups/{id}");
        return response.IsSuccessStatusCode;
    }
}