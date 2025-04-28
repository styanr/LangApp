using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.Assignments.Models;
using LangApp.Api.Endpoints.Posts.Models;
using LangApp.Application.Assignments.Commands;
using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Queries;
using LangApp.Application.Common;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.Assignments;

public class AssignmentsModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("assignments").WithTags("Assignments");
        group.MapPost("/multiple-choice/", CreateMultipleChoice).WithName("CreateMultipleChoiceAssignment");
        group.MapPost("/fill-in-the-blank/", CreateFillInTheBlank).WithName("CreateFillInTheBlankAssignment");
        group.MapPost("/pronunciation/", CreatePronunciation).WithName("CreatePronunciationAssignment");

        group.MapGet("/{id:guid}", Get).WithName("GetAssignment");

        app.MapVersionedGroup("groups").WithTags("Assignments").MapGet("/{groupId:guid}/assignments", GetByGroup)
            .WithName("GetAssignmentsByGroup");
    }

    private async Task<Results<Ok<AssignmentDto>, NotFound>> Get(
        [AsParameters] GetAssignmentRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetAssignment(request.Id, userId);
        var assignment = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(assignment);
    }


    private async Task<Results<Ok<PagedResult<AssignmentDto>>, NotFound>> GetByGroup(
        [AsParameters] GetAssignmentByGroupRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetAssignmentByGroup(request.GroupId, userId);
        var assignment = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(assignment);
    }

    private async Task<CreatedAtRoute> CreateMultipleChoice(
        [FromBody] CreateMultipleChoiceAssignmentRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();

        var command = new CreateMultipleChoiceAssignment(
            userId,
            request.GroupId,
            request.DueTime,
            request.MaxScore,
            request.Details
        );

        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetAssignment", new { id });
    }

    private async Task<CreatedAtRoute> CreateFillInTheBlank(
        [FromBody] CreateFillInTheBlankAssignmentRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();

        var command = new CreateFillInTheBlankAssignment(
            userId,
            request.GroupId,
            request.DueTime,
            request.MaxScore,
            request.Details
        );

        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetAssignment", new { id });
    }

    private async Task<CreatedAtRoute> CreatePronunciation(
        [FromBody] CreatePronunciationAssignmentRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();

        var command = new CreatePronunciationAssignment(
            userId,
            request.GroupId,
            request.DueTime,
            request.MaxScore,
            request.Details
        );

        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetAssignment", new { id });
    }
}
