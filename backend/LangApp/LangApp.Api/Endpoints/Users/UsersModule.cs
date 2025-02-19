using LangApp.Api.Common.Endpoints;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Users.Commands;
using LangApp.Application.Users.Dto;
using LangApp.Application.Users.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.Users;

public class UsersModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("users");
        group.MapGet("{id:guid}", Get).WithName("GetUser");
        group.MapPost("/", Create).WithName("CreateUser");
    }

    private async Task<Results<Ok<UserDto>, NotFound>> Get(
        [AsParameters] GetUser query,
        [FromServices] IQueryDispatcher dispatcher
    )
    {
        var user = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(user);
    }

    private async Task<CreatedAtRoute> Create(
        [FromBody] CreateUser command,
        [FromServices] ICommandDispatcher dispatcher
    )
    {
        var id = await dispatcher.DispatchAsync<CreateUser, Guid>(command);

        return TypedResults.CreatedAtRoute("CreateUser", new { id });
    }
}