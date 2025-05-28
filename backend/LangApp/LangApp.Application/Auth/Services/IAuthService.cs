using LangApp.Application.Auth.Models;
using LangApp.Core.Entities.Users;

namespace LangApp.Application.Auth.Services;

public interface IAuthService
{
    Task<TokenResponse?> Authenticate(string username, string password);
    Task<TokenResponse> RefreshToken(string refreshToken);
    Task<string?> RequestPasswordReset(string email);
    Task ResetPasswordAsync(string email, string token, string password);
    Task<string> GenerateEmailConfirmationTokenAsync(string email);
    Task ConfirmEmailAsync(string email, string token);
    Task CreateUserAsync(ApplicationUser user, string password);
}
