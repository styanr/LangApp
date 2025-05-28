using LangApp.Api.Common.Endpoints;
using LangApp.Api.Common.Services;
using LangApp.Application.Auth.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace LangApp.Api.Endpoints.Client;

public class ClientModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("client").WithTags("Client");
        group.MapGet("/deep-link", RedirectToClientApp)
            .WithName("RedirectToClientApp")
            .AllowAnonymous();
    }

    private IResult RedirectToClientApp(
        [FromServices] IOptions<ClientAppOptions> deepLinkOptions,
        [FromServices] ILogger<ClientModule> logger,
        [FromServices] IHtmlTemplateService htmlTemplateService,
        HttpContext context)
    {
        var queryString = context.Request.QueryString.Value;
        if (string.IsNullOrWhiteSpace(queryString))
        {
            logger.LogWarning("Cannot redirect to client app without a token");
            return Results.BadRequest("Query string is required");
        }

        try
        {
            string appScheme = deepLinkOptions.Value.AppScheme ?? "testapp";
            string path = "auth/reset-password";
            string deepLink = $"{appScheme}://{path}{queryString}";

            logger.LogInformation("Attempting to redirect to deep link: {DeepLink}", deepLink);
            
            return Results.Content(
                htmlTemplateService.RenderDeepLinkRedirect(deepLink, deepLinkOptions.Value),
                "text/html");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing redirect for link: {QueryString}", queryString);
            return Results.BadRequest("Invalid request");
        }
    }
}