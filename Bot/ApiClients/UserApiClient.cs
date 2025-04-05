using System.Net;
using System.Net.Http.Json;
using Core.DTO;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace PIBScheduleBot.ApiClients;

public class UserApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public UserApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ApiKey"];
    }

    public async Task<List<User>?> GetUsersAsync()
    {
        var response = await _httpClient.GetAsync($"api/users");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<List<User>>();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/users/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<User?> GetUserByTelegramIdAsync(string telegramId)
    {
        var response = await _httpClient.GetAsync($"api/users/telegram/{telegramId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserStatusAsync(string telegramId, UserStatus status)
    {
        var request = new UpdateUserStatusRequest{TelegramId = telegramId, UserStatus = status};
        var response = await _httpClient.PutAsJsonAsync("api/users/status", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserGroupAsync(string telegramId, int groupId)
    {
        var request = new UpdateUserGroupRequest { TelegramId = telegramId, GroupId = groupId };
        var response = await _httpClient.PutAsJsonAsync("api/users/group", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserTeacherAsync(string telegramId, int teacherId)
    {
        var request = new UpdateUserTeacherRequest {TelegramId = telegramId, TeacherId = teacherId};
        var response = await _httpClient.PutAsJsonAsync("api/users/teacher", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserByIdAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/users/{id}");
        return response.IsSuccessStatusCode;
    }
}