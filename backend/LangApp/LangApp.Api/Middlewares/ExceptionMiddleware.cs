using System.Text.Json;
using LangApp.Application.Auth.Exceptions;
using LangApp.Application.Common.Exceptions;
using LangApp.Core.Common.Exceptions;
using LangApp.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace LangApp.Api.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (InvalidCredentialsException e)
        {
            await CreateResponse(context, StatusCodes.Status401Unauthorized, e);
        }
        catch (UnauthorizedException e)
        {
            await CreateResponse(context, StatusCodes.Status403Forbidden, e);
        }
        catch (NotFoundException e)
        {
            await CreateResponse(context, StatusCodes.Status404NotFound, e);
        }
        catch (ValidationException e)
        {
            await CreateResponse(context, StatusCodes.Status400BadRequest, e,
                e => new() { { "validation_errors", e.Errors } }
            );
        }
        catch (Exception e) when (e is LangAppException or BadHttpRequestException)
        {
            await CreateResponse(context, StatusCodes.Status400BadRequest, e);
        }
        catch (Exception e)
        {
            await CreateResponse(context, StatusCodes.Status500InternalServerError, e);
        }
    }

    private static async Task CreateResponse<TException>(
        HttpContext context,
        int statusCode,
        TException e,
        Func<TException, Dictionary<string, object>>? func = null
    ) where TException : Exception
    {
        context.Response.StatusCode = statusCode;
        context.Response.Headers.Append(HeaderNames.ContentType, "application/json");

        var errorCode = ToUnderscoreCase(e.GetType().Name.Replace("Exception", string.Empty));

        var problemDetails = new ProblemDetails
        {
            Type = MdnErrorUrl(statusCode),
            Title = e.GetType().Name.Replace("Exception", string.Empty),
            Status = statusCode,
            Detail = e.Message,
            Instance = context.Request.Path,
            Extensions = { { "error_code", errorCode } }
        };

        if (func is not null)
        {
            foreach (var (key, value) in func(e))
            {
                problemDetails.Extensions.Add(key, value);
            }
        }

        var json = JsonSerializer.Serialize(problemDetails,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

        await context.Response.WriteAsync(json);
    }

    private static string ToUnderscoreCase(string value)
        => string.Concat(value.Select((x, i) =>
            i > 0 && char.IsUpper(x) && char.IsLower(value[i - 1]) ? $"_{x}" : x.ToString())).ToLower();

    private static string MdnErrorUrl(int code) =>
        $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{code}";
}