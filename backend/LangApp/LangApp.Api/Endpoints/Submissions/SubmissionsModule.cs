using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.Submissions.Models;
using LangApp.Application.Common;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Submissions.Commands;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.Submissions;

public class SubmissionsModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("submissions").WithTags("Submissions");
        group.MapGet("{id:guid}", Get).WithName("GetSubmission");

        var assignmentGroup = app.MapVersionedGroup("assignments/{assignmentId:guid}/submissions")
            .WithTags("Submissions");
        assignmentGroup.MapPost("/", Create).WithName("CreateAssignmentSubmission");
        assignmentGroup.MapGet("", GetByAssignment).WithName("GetSubmissionsByAssignment");
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
}
