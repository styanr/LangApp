using LangApp.Api.Common.Endpoints;
using LangApp.Api.Common.Services;
using LangApp.Application.Auth.Commands;
using LangApp.Application.Auth.Models;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Users.Dto;
using LangApp.Application.Users.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.Auth;

public class AuthModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("auth").WithTags("Authentication");
        group.MapPost("/register", Register)
            .AllowAnonymous()
            .WithName("Register");
        group.MapPost("/login", Login)
            .AllowAnonymous()
            .WithName("Login");
        group.MapPost("/refresh", Refresh)
            .AllowAnonymous()
            .WithName("Refresh");
        group.MapPost("/request-password-reset", RequestPasswordReset)
            .AllowAnonymous()
            .WithName("RequestPasswordReset")
            .WithDescription(
                "Request a password reset. The user will receive an email with a link to the client app reset their password.");
        group.MapPost("/reset-password", ResetPassword)
            .AllowAnonymous()
            .WithName("ResetPassword");
        group.MapGet("/confirm-email", ConfirmEmail)
            .AllowAnonymous()
            .WithName("ConfirmEmail")
            .WithDescription(
                "Confirm user's email address using the token received in the confirmation email.");
    }


    private async Task<CreatedAtRoute> Register(
        [FromBody] Register command,
        [FromServices] ICommandDispatcher dispatcher
    )
    {
        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetUser", new { id });
    }

    private async Task<Ok<TokenResponse>> Login(
        [FromBody] Login command,
        [FromServices] ICommandDispatcher dispatcher
    )
    {
        var token = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.Ok(token);
    }

    private async Task<Ok<TokenResponse>> Refresh(
        [FromBody] Refresh command,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var tokenResponse = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.Ok(tokenResponse);
    }

    private async Task<NoContent> RequestPasswordReset(
        [FromBody] RequestPasswordReset command,
        [FromServices] ICommandDispatcher dispatcher
    )
    {
        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }

    private async Task<Ok> ResetPassword(
        [FromBody] ResetPassword command,
        [FromServices] ICommandDispatcher dispatcher
    )
    {
        await dispatcher.DispatchAsync(command);

        return TypedResults.Ok();
    }

    private async Task<Results<ContentHttpResult, ProblemHttpResult>> ConfirmEmail(
        [FromQuery] string email,
        [FromQuery] string token,
        [FromServices] ICommandDispatcher dispatcher,
        [FromServices] IHtmlTemplateService htmlTemplateService
    )
    {
        try
        {
            await dispatcher.DispatchAsync(new ConfirmEmail(email, token));
            return TypedResults.Content(htmlTemplateService.RenderEmailConfirmationSuccess(), "text/html");
        }
        catch (Exception ex)
        {
            return TypedResults.Content(htmlTemplateService.RenderEmailConfirmationError(), "text/html");
        }
    }
}
