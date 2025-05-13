using System.Net;
using LangApp.Application.Auth.Options;
using Microsoft.Extensions.Options;

namespace LangApp.Infrastructure.Email.DeepLinks;

public class DeepLinkGenerator : IDeepLinkGenerator
{
    private readonly IOptions<PublicOptions> _publicOptions;

    public DeepLinkGenerator(IOptions<PublicOptions> publicOptions)
    {
        _publicOptions = publicOptions;
    }

    public string GenerateResetPasswordLink(string email, string token)
    {
        var apiUrl = _publicOptions.Value.BaseUrl;

        var encodedToken = WebUtility.UrlEncode(token);
        var encodedEmail = WebUtility.UrlEncode(email);

        return $"{apiUrl}/client/deep-link?link=reset-password?token={encodedToken}&email={encodedEmail}";
    }
}
