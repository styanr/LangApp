namespace LangApp.Application.Auth.Models;

public record TokenResponse(string AccessToken, string RefreshToken)
{
    public string AccessToken { get; init; } = AccessToken;
    public string RefreshToken { get; init; } = RefreshToken;
}
