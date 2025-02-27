using LangApp.Application.Auth.Models;
using LangApp.Core.Entities.Users;

namespace LangApp.Application.Auth.Services;

public interface IAuthService
{
    Task<TokenResponse?> Authenticate(string username, string password);
}