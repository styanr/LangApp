using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.Submissions.Models;
using LangApp.Application.Common;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Submissions.Commands;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Queries;
using LangApp.Core.ValueObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.Submissions;

public class SubmissionsModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("submissions").WithTags("Submissions");
        group.MapGet("{id:guid}", Get).WithName("GetSubmission");
        group.MapPut("{submissionId:guid}/activities/{activityId:guid}", EditSubmissionGrade)
            .WithName("EditSubmissionGrade");

        var assignmentGroup = app.MapVersionedGroup("assignments/{assignmentId:guid}/submissions")
            .WithTags("Submissions");
        assignmentGroup.MapPost("/", Create).WithName("CreateAssignmentSubmission");
        assignmentGroup.MapGet("", GetByAssignment).WithName("GetSubmissionsByAssignment");

        app.MapVersionedGroup("groups/{groupId:guid}/submissions")
            .WithTags("Submissions")
            .MapGet("", GetSubmissionsByUserGroup)
            .WithName("GetSubmissionsByUserGroup");

        app.MapVersionedGroup("assignments/{assignmentId:guid}").WithTags("Submissions")
            .MapPost("activities/{activityId:guid}/evaluate-pronunciation", EvaluatePronunciationSubmission)
            .WithName("EvaluatePronunciationSubmission")
            .WithDescription(
                "This endpoint is used to evaluate pronunciation before actually submitting the assignment. " +
                "It is used for receiving feedback to the student and giving them the opportunity to improve their pronunciation.");
    }

    private async Task<Results<Ok<AssignmentSubmissionDto>, NotFound>> Get(
        [AsParameters] GetSubmissionRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetSubmission(request.Id, userId);
        var submission = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(submission);
    }

    private async Task<Results<Ok<PagedResult<AssignmentSubmissionDto>>, NotFound>> GetByAssignment(
        [AsParameters] GetSubmissionsByAssignmentRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        var userId = context.User.GetUserId();
        var query = new GetSubmissionsByAssignment(request.AssignmentId, userId)
        {
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
        };
        var submission = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(submission);
    }

    private async Task<CreatedAtRoute> Create(
        [FromRoute] Guid assignmentId,
        [FromBody] CreateAssignmentSubmissionRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new CreateAssignmentSubmission(
            assignmentId,
            userId,
            request.ActivitySubmissionDtos
        );

        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetSubmission", new { id });
    }

    private async Task<NoContent> EditSubmissionGrade(
        [FromRoute] Guid submissionId,
        [FromRoute] Guid activityId,
        [FromBody] SubmissionGradeDto grade,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context
    )
    {
        var userId = context.User.GetUserId();
        var command = new EditActivitySubmissionGrade(
            submissionId,
            activityId,
            grade,
            userId
        );

        await dispatcher.DispatchAsync(command);
        return TypedResults.NoContent();
    }

    private async Task<SubmissionGradeDto> EvaluatePronunciationSubmission(
        [FromRoute] Guid assignmentId,
        [FromRoute] Guid activityId,
        [FromBody] EvaluatePronunciationSubmissionRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context
    )
    {
        var userId = context.User.GetUserId();

        var command = new EvaluatePronunciation(request.FileUri, assignmentId, activityId, userId);
        var result = await dispatcher.DispatchWithResultAsync(command);
        return result;
    }

    private async Task<Results<Ok<PagedResult<UserGroupSubmissionDto>>, NotFound>> GetSubmissionsByUserGroup(
        [FromRoute] Guid groupId,
        int? pageNumber,
        int? pageSize,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetSubmissionsByUserGroup(groupId, userId)
        {
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
        };
        var submission = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(submission);
    }
}