using System.Net.Http.Headers;
using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.StudyGroups.Models;
using LangApp.Application.Common;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Commands;
using LangApp.Application.StudyGroups.Dto;
using LangApp.Application.StudyGroups.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.StudyGroups;

public class StudyGroupsModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("groups");

        group.MapGet("/{id:guid}", Get)
            .WithName("GetStudyGroup");
        group.MapPost("/", Create).WithName("CreateStudyGroup");
        group.MapPost("/{id:guid}/members", AddMembers).WithName("AddMembersToStudyGroup");
        group.MapDelete("/{id:guid}/members", RemoveMembers).WithName("RemoveMembersFromStudyGroup");
        group.MapPut("/{id:guid}", Put).WithName("UpdateStudyGroupInfo");

        app.MapVersionedGroup("users").MapGet("/{userId:guid}/groups", GetByUser).WithName("GetStudyGroupByUser");
    }

    private async Task<Results<Ok<StudyGroupDto>, NotFound>> Get(
        [AsParameters] GetStudyGroup query,
        [FromServices] IQueryDispatcher dispatcher)
    {
        var result = await dispatcher.QueryAsync(query);
        return ApplicationTypedResults.OkOrNotFound(result);
    }

    private async Task<Results<Ok<PagedResult<StudyGroupSlimDto>>, NotFound>> GetByUser(
        [AsParameters] GetStudyGroupsByUser query,
        [FromServices] IQueryDispatcher dispatcher)
    {
        var result = await dispatcher.QueryAsync(query);
        return ApplicationTypedResults.OkOrNotFound(result);
    }

    private async Task<CreatedAtRoute> Create(
        [FromBody] CreateStudyGroup command,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetStudyGroup", new { id });
    }

    private async Task<NoContent> AddMembers(
        [FromRoute] Guid id,
        [FromBody] MembersBodyRequestModel request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var command = new AddMembersToStudyGroup(
            id,
            request.Members
        );

        await dispatcher.DispatchAsync(command);
        return TypedResults.NoContent();
    }

    private async Task<NoContent> RemoveMembers(
        [FromRoute] Guid id,
        [FromBody] MembersBodyRequestModel request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var command = new RemoveMembersFromStudyGroup(
            id,
            request.Members
        );

        await dispatcher.DispatchAsync(command);
        return TypedResults.NoContent();
    }

    private async Task<NoContent> Put(
        [FromRoute] Guid id,
        [FromBody] StudyGroupInfoRequestModel request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var command = new UpdateStudyGroupInfo(
            id,
            request.Name
        );

        await dispatcher.DispatchAsync(command);
        return TypedResults.NoContent();
    }
}