using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.Users.Models;
using LangApp.Application.Common;
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
        var group = app.MapVersionedGroup("users").WithTags("Users");
        group.MapGet("{id:guid}", Get).WithName("GetUser");
        group.MapGet("", Search).WithName("SearchUsers");
        group.MapGet("me", GetAuthenticated).WithName("GetCurrentUser");
        group.MapPut("me", UpdateInfo).WithName("UpdateUserInfo");
    }

    private async Task<Results<Ok<UserDto>, NotFound>> GetAuthenticated(
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context
    )
    {
        var userId = context.User.GetUserId();
        var query = new GetUser(userId);
        var user = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(user);
    }

    private async Task<Results<Ok<UserDto>, NotFound>> Get(
        [AsParameters] GetUserRequest request,
        [FromServices] IQueryDispatcher dispatcher
    )
    {
        var query = new GetUser(request.Id);
        var user = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(user);
    }

    private async Task<Results<Ok<PagedResult<UserDto>>, NotFound>> Search(
        [AsParameters] SearchUsersRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new SearchUsers(request.SearchTerm, userId);

        var result = await dispatcher.QueryAsync(query);
        return ApplicationTypedResults.OkOrNotFound(result);
    }

    private async Task<NoContent> UpdateInfo(
        [FromBody] UpdateUserInfoRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context
    )
    {
        var userId = context.User.GetUserId();

        var command = new UpdateUserInfo(
            userId,
            request.Username,
            request.FullName,
            request.PictureUrl
        );

        await dispatcher.DispatchAsync(command);
        return TypedResults.NoContent();
    }
}
