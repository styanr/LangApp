using LangApp.Application.Auth.Models;
using LangApp.Infrastructure.EF.Identity;

namespace LangApp.Infrastructure.EF.Services;

public interface ITokenFactory
{
    TokenResponse GenerateJwtToken(IdentityApplicationUser user);
}