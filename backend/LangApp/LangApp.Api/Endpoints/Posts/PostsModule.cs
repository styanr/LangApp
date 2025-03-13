using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.Posts.Models;
using LangApp.Application.Common;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Commands;
using LangApp.Application.Posts.Dto;
using LangApp.Application.Posts.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.Posts;

public class PostsModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("posts").WithTags("Posts");

        group.MapGet("/{id:guid}", Get).WithName("GetPost");
        group.MapPost("/", Create).WithName("CreatePost");
        group.MapPut("/{id:guid}", Edit).WithName("EditPost");
        group.MapPatch("/{id:guid}", Archive).WithName("ArchivePost");

        app.MapVersionedGroup("groups")
            .WithTags("Posts")
            .MapGet("/{groupId:guid}/posts", GetByGroup)
            .WithName("GetPostsByGroup");
    }

    private async Task<Results<Ok<PostDto>, NotFound>> Get(
        [AsParameters] GetPostRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetPost(request.Id, userId);
        var post = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(post);
    }

    private async Task<Results<Ok<PagedResult<PostSlimDto>>, NotFound>> GetByGroup(
        [AsParameters] GetPostsByGroupRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetPostsByGroup(request.GroupId, userId);
        var posts = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(posts);
    }

    private async Task<CreatedAtRoute> Create(
        [FromBody] CreatePostRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new CreatePost(
            userId,
            request.GroupId,
            request.Type,
            request.Title,
            request.Content,
            request.Media);

        var id = await dispatcher.DispatchWithResultAsync(command);

        return TypedResults.CreatedAtRoute("GetPost", new { id });
    }

    private async Task<NoContent> Edit(
        [FromRoute] Guid id,
        [FromBody] EditPostRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new EditPost(id, request.Content, userId);

        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }

    private async Task<NoContent> Archive(
        [AsParameters] ArchivePostRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new ArchivePost(request.Id, userId);
        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }
}