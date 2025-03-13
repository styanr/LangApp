using LangApp.Api.Common.Endpoints;
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

        var userGroup = app.MapVersionedGroup("users").WithTags("Users");
        userGroup.MapGet("{id:guid}", Get).WithName("GetUser");
        userGroup.MapGet("me", GetAuthenticated).WithName("GetCurrentUser");
    }

    private async Task<Results<Ok<UserDto>, NotFound>> GetAuthenticated(
        [AsParameters] GetUser query,
        [FromServices] IQueryDispatcher dispatcher
    )
    {
        var user = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(user);
    }

    private async Task<Results<Ok<UserDto>, NotFound>> Get(
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context
    )
    {
        var userId = context.User.GetUserId();

        var query = new GetUser(userId);
        var user = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(user);
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
}