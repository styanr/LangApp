using LangApp.Api.Common.Endpoints;
using LangApp.Application.Auth.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

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
            var html = $@"
<!DOCTYPE html>
<html lang='en'>
  <head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Opening App...</title>
    <style>
      body {{ font-family: Arial, sans-serif; text-align: center; margin-top: 50px; padding: 20px; }}
      .loader {{ border: 4px solid #f3f3f3; border-top: 4px solid #3498db; border-radius: 50%; width: 30px; height: 30px; animation: spin 1s linear infinite; margin: 20px auto; }}
      @keyframes spin {{ 0% {{ transform: rotate(0deg); }} 100% {{ transform: rotate(360deg); }} }}
      .container {{ max-width: 600px; margin: 0 auto; }}
    </style>
  </head>
  <body>
    <div class='container'>
      <h2>Opening the application...</h2>
      <div class='loader'></div>
      <p>If the app doesn't open automatically, please click the button below:</p>
      <p><a href='{deepLink}' style='display: inline-block; background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px;'>Open App</a></p>
      
      <div id='fallbackMessage' style='display: none; margin-top: 30px; padding: 15px; border: 1px solid #ddd; border-radius: 4px;'>
        <h3>App not installed?</h3>
        <p>If you don't have the app installed, you can download it from:</p>
        <p>
          <a href='{deepLinkOptions.Value.PlayStoreUrl ?? "#"}' style='display: inline-block; background-color: #FF5722; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px;'>Google Play</a>
        </p>
      </div>
    </div>

    <script>
      // Try to open the app immediately
      window.location.href = '{deepLink}';
      
      // Set a timeout to show the fallback message if the app doesn't open
      setTimeout(function() {{
        document.getElementById('fallbackMessage').style.display = 'block';
      }}, 2000);
    </script>
  </body>
</html>";

            return Results.Content(html, "text/html");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing redirect for link: {QueryString}", queryString);
            return Results.BadRequest("Invalid request");
        }
    }
}