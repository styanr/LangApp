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

public class Auth : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("auth");
        group.MapGet("{id:guid}", Get).WithName("GetUser");
        group.MapPost("/register", Register)
            .AllowAnonymous()
            .WithName("Register");
        group.MapPost("/login", Login)
            .AllowAnonymous()
            .WithName("Login");
    }

    private async Task<Results<Ok<UserDto>, NotFound>> Get(
        [AsParameters] GetUser query,
        [FromServices] IQueryDispatcher dispatcher
    )
    {
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