using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LangApp.Application.Auth.Exceptions;
using LangApp.Application.Auth.Models;
using LangApp.Application.Auth.Services;
using LangApp.Core.Entities.Users;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Identity;
using LangApp.Infrastructure.EF.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LangApp.Infrastructure.EF.Services;

public class TokenFactory : ITokenFactory
{
    private readonly UserManager<IdentityApplicationUser> _userManager;
    private readonly JwtOptions _options;

    public TokenFactory(UserManager<IdentityApplicationUser> userManager, IOptions<JwtOptions> options)
    {
        _userManager = userManager;
        _options = options.Value;
    }

    public string GenerateAccessToken(IdentityApplicationUser user)
    {
        var secret = Encoding.UTF8.GetBytes(_options.Secret);

        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity([
                new("sub", user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName!), // this should never be null
                new(ClaimTypes.NameIdentifier, user.Email!),
                new(ClaimTypes.Role, user.Role.GetName())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_options.Expiry),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
