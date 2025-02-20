using LangApp.Api.Common.Endpoints;
using LangApp.Application.Common;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Commands;
using LangApp.Application.StudyGroups.Dto;
using LangApp.Application.StudyGroups.Queries;
using LangApp.Core.Services.KeyGeneration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.StudyGroups;

public class StudyGroupsModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("groups");

        group.MapGet("/{id:guid}", Get).WithName("GetStudyGroup");
        group.MapPost("/", Create).WithName("CreateStudyGroup");

        app.MapGet("/users/{userId:guid}/groups", GetByUser).WithName("GetStudyGroupByUser");
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
        [FromBody] AddMembersToStudyGroup command,
        [FromServices] ICommandDispatcher dispatcher)
    {
        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }
}