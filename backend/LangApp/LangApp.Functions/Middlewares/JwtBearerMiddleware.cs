using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using LangApp.Functions.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LangApp.Functions.Middlewares;

// https://github.com/forrestcoward/AzureFunctionJWTMiddleware/blob/main/Function/TokenValidationMiddleware.cs
public class JwtBearerMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpRequest = await context.GetHttpRequestDataAsync();

        if (httpRequest == null)
        {
            // Not an HTTP trigger.
            await next(context);
            return;
        }

        var logger = context.GetLogger<JwtBearerMiddleware>();
        logger.LogInformation("Authorizing request...");

        // Check if the Authenticate attribute is present.
        var authorizeAttribute = GetTargetFunctionMethod(context)
            .GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        var shouldAuthorize = authorizeAttribute != null;

        var authorized = false;
        if (!shouldAuthorize)
        {
            logger.LogInformation($"No AuthorizeAttribute found on function.");
            authorized = true;
        }

        if (shouldAuthorize)
        {
            if (httpRequest.Headers.TryGetValues("Authorization", out var values))
            {
                var token = values.First().Split(" ").Last();
                authorized = ValidateToken(token, out var principal, logger);
            }
            else
            {
                logger.LogInformation($"Missing authorization token on request.");
            }
        }

        if (!authorized)
        {
            logger.LogInformation("Authorization failed!");
            context.Items["HttpResponseData"] = await CreateUnauthorizedResponse(httpRequest);
        }
        else
        {
            logger.LogInformation("Request authorized.");

            await next(context);
        }
    }

    private static bool ValidateToken(string token, out ClaimsPrincipal? principal, ILogger logger)
    {
        Environment.GetEnvironmentVariables();

        principal = null;
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? ""))
        };

        try
        {
            principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
            return true;
        }
        catch (SecurityTokenException se)
        {
            logger.LogError($"[Authentication failed] Token validation failed: {se.Message}");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError($"[Authentication failed] Unexpected error during token validation: {ex.Message}");
            return false;
        }
    }

    private async Task<HttpResponseData> CreateUnauthorizedResponse(
        HttpRequestData request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = request.CreateResponse(HttpStatusCode.Unauthorized);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync("Unauthorized");
        return response;
    }

    private static MethodInfo GetTargetFunctionMethod(FunctionContext context)
    {
        var assemblyPath = context.FunctionDefinition.PathToAssembly;
        var assembly = Assembly.LoadFrom(assemblyPath);
        var typeName =
            context.FunctionDefinition.EntryPoint.Substring(0, context.FunctionDefinition.EntryPoint.LastIndexOf('.'));
        var type = assembly.GetType(typeName);
        var methodName =
            context.FunctionDefinition.EntryPoint.Substring(context.FunctionDefinition.EntryPoint.LastIndexOf('.') + 1);
        var method = type.GetMethod(methodName);
        return method;
    }
}
