using LangApp.Application.Auth.Models;
using LangApp.Application.Auth.Services;
using LangApp.Core.Entities.Users;
using LangApp.Infrastructure.EF.Identity;
using Microsoft.AspNetCore.Identity;

namespace LangApp.Infrastructure.EF.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityApplicationUser> _userManager;
    private readonly SignInManager<IdentityApplicationUser> _signInManager;
    private readonly ITokenFactory _tokenFactory;

    public AuthService(UserManager<IdentityApplicationUser> userManager,
        SignInManager<IdentityApplicationUser> signInManager, ITokenFactory tokenFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenFactory = tokenFactory;
    }


    public async Task<TokenResponse?> Authenticate(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null) return null;

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        return !result.Succeeded ? null : _tokenFactory.GenerateJwtToken(user);
    }
}