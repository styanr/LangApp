using System.Collections.Specialized;
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


        group.MapPost("/{postId:guid}/comments", CreateComment).WithName("CreatePostComment");
        group.MapPut("/{postId:guid}/comments/{commentId:guid}", UpdateComment).WithName("UpdatePostComment");
        group.MapDelete("/{postId:guid}/comments/{commentId:guid}", DeleteComment).WithName("DeletePostComment");

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
        var command = new EditPost(id, request.Content, request.Media, userId);

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

    private async Task<CreatedAtRoute> CreateComment(
        [FromRoute] Guid postId,
        [FromBody] CreatePostCommentRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new CreatePostComment(userId, postId, request.Content);
        await dispatcher.DispatchWithResultAsync(command);

        return TypedResults.CreatedAtRoute("GetPost", new { id = postId });
    }

    private async Task<Results<NoContent, NotFound>> UpdateComment(
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId,
        [FromBody] EditPostCommentRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new EditPostComment(postId, commentId, request.Content, userId);
        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }

    private async Task<Results<NoContent, NotFound>> DeleteComment(
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new DeletePostComment(postId, commentId, userId);

        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }
}
