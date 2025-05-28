using System.Net;
using LangApp.Application.Auth.Options;
using Microsoft.AspNetCore.WebUtilities;
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

        var queryParams = new Dictionary<string, string?>
        {
            { "email", email },
            { "token", token }
        };

        var query = QueryHelpers.AddQueryString($"{apiUrl}/client/deep-link", queryParams);
        return query;
    }

    public string GenerateConfirmationLink(string email, string token)
    {
        var apiUrl = _publicOptions.Value.BaseUrl;

        var queryParams = new Dictionary<string, string?>
        {
            { "email", email },
            { "token", token }
        };

        var query = QueryHelpers.AddQueryString($"{apiUrl}/auth/confirm-email", queryParams);
        return query;
    }
}
