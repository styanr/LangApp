using LangApp.Application.Auth.Exceptions;
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

        // Check if email is confirmed
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            throw new ValidationException(new[] { "Please confirm your email before signing in." });
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (!result.Succeeded) return null;

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _tokenFactory.GenerateRefreshToken(),
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
            throw new InvalidCredentialsException("Refresh token expired");
        }

        var user = token.User;
        
        // Check if email is still confirmed (in case it was unconfirmed after initial confirmation)
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            throw new ValidationException(new[] { "Email confirmation is required. Please confirm your email." });
        }

        var accessToken = _tokenFactory.GenerateAccessToken(user);
        token.Token = _tokenFactory.GenerateRefreshToken();
        token.ExpiresAtUtc = DateTime.UtcNow + TimeSpan.FromDays(30);
        await _context.SaveChangesAsync();

        return new TokenResponse(accessToken, token.Token);
    }

    public async Task<string?> RequestPasswordReset(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return null;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return token;
    }

    public async Task ResetPasswordAsync(string email, string token, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return;

        var result = await _userManager.ResetPasswordAsync(user, token, password);

        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.Select(e => e.Description));
        }
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new ValidationException(new[] { "User not found." });
        }

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new ValidationException(new[] { "User not found." });
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.Select(e => e.Description));
        }
    }

    public async Task CreateUserAsync(ApplicationUser user, string password)
    {
        var identityUser = new IdentityApplicationUser(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.PictureUrl,
            user.Role
        );

        var result = await _userManager.CreateAsync(identityUser, password);
        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.Select(e => e.Description));
        }
    }
}
