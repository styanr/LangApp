using System.Net.Http.Headers;
using System.Net.Http.Json;
using LangApp.Application.Auth.Commands;
using LangApp.Application.Auth.Models;
using LangApp.Application.Users.Dto;
using LangApp.Application.Users.Models;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Context;
using Newtonsoft.Json;

namespace LangApp.Tests.Integration.Helpers;

internal class TestUserHelper
{
    private readonly HttpClient _client;
    private readonly WriteDbContext _context;

    public TestUserHelper(HttpClient client, WriteDbContext context)
    {
        _client = client;
        _context = context;
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
        var body = await regResponse.Content.ReadAsStringAsync();
        Console.WriteLine(body);
        regResponse.EnsureSuccessStatusCode();

        var user = _context.Users.FirstOrDefault(u => u.UserName == username);
        if (user is null)
        {
            throw new InvalidOperationException("User not found after registration.");
        }

        user.EmailConfirmed = true;
        await _context.SaveChangesAsync();

        return await LoginAsync(username, password);
    }

    public async Task<(string Token, Guid UserId)> LoginAsync(string username, string password)
    {
        var login = new Login(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
        var body = await loginResponse.Content.ReadAsStringAsync();
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