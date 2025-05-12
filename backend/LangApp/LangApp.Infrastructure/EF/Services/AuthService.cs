using LangApp.Application.Auth.Models;
using LangApp.Application.Auth.Services;
using LangApp.Application.Common.Exceptions;
using LangApp.Core.Entities.Users;
using LangApp.Core.Exceptions;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Services;

internal class AuthService : IAuthService
{
    private readonly UserManager<IdentityApplicationUser> _userManager;
    private readonly SignInManager<IdentityApplicationUser> _signInManager;
    private readonly ITokenFactory _tokenFactory;
    private readonly WriteDbContext _context;

    public AuthService(UserManager<IdentityApplicationUser> userManager,
        SignInManager<IdentityApplicationUser> signInManager, ITokenFactory tokenFactory, WriteDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenFactory = tokenFactory;
        _context = context;
    }


    public async Task<TokenResponse?> Authenticate(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null) return null;

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (!result.Succeeded) return null;

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _tokenFactory.GenerateRefreshToken(),
            // TODO: Get from config
            ExpiresAtUtc = DateTime.UtcNow + TimeSpan.FromDays(30)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var accessToken = _tokenFactory.GenerateAccessToken(user);

        return new TokenResponse(accessToken, refreshToken.Token);
    }

    public async Task<TokenResponse> RefreshToken(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token is null || token.IsRevoked || token.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new LangAppException("Refresh token expired");
        }

        var user = token.User;
        var accessToken = _tokenFactory.GenerateAccessToken(user);
        token.Token = _tokenFactory.GenerateRefreshToken();
        token.ExpiresAtUtc = DateTime.UtcNow + TimeSpan.FromDays(30);
        await _context.SaveChangesAsync();

        return new TokenResponse(accessToken, token.Token);
    }
}
