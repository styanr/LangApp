using LangApp.Application.Auth.Models;
using LangApp.Infrastructure.EF.Identity;

namespace LangApp.Infrastructure.EF.Services;

public interface ITokenFactory
{
    string GenerateAccessToken(IdentityApplicationUser user);
    public string GenerateRefreshToken();
}
