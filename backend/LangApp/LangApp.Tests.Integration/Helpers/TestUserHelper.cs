using System.Net.Http.Headers;
using System.Net.Http.Json;
using LangApp.Application.Auth.Commands;
using LangApp.Application.Auth.Models;
using LangApp.Application.Users.Dto;
using LangApp.Application.Users.Models;
using LangApp.Core.Enums;
using Newtonsoft.Json;

namespace LangApp.Tests.Integration.Helpers;

public class TestUserHelper
{
    private readonly HttpClient _client;

    public TestUserHelper(HttpClient client)
    {
        _client = client;
    }

    public async Task<(string Token, Guid UserId)> RegisterAndLoginAsync(
        string username,
        string password,
        UserRole role = UserRole.Teacher)
    {
        var register = new Register(
            username,
            $"{username}@test.com",
            new FullNameModel("Test", "User"),
            null,
            role,
            password
        );

        var regResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
        regResponse.EnsureSuccessStatusCode();

        var login = new Login(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
        loginResponse.EnsureSuccessStatusCode();

        var content = await loginResponse.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content)!;

        var authHeader = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        _client.DefaultRequestHeaders.Authorization = authHeader;

        var meResponse = await _client.GetAsync("/api/v1/users/me");
        meResponse.EnsureSuccessStatusCode();

        var user = JsonConvert.DeserializeObject<UserDto>(
            await meResponse.Content.ReadAsStringAsync()
        )!;

        return (tokenResponse.AccessToken, user.Id);
    }
}