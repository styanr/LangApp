namespace LangApp.Application.Auth.Models;

public record TokenResponse(string Token)
{
    public string Token { get; init; } = Token;
}
