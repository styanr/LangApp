using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.Submissions.Models;
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
        group.MapPost("/multiple-choice/", CreateMultipleChoice).WithName("CreateMultipleChoiceSubmission");
        group.MapGet("{id:guid}", Get).WithName("GetSubmission");
    }

    private async Task<Results<Ok<SubmissionDto>, NotFound>> Get(
        [AsParameters] GetSubmissionRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetSubmission(request.Id, userId);
        var submission = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(submission);
    }

    private async Task<CreatedAtRoute> CreateMultipleChoice(
        [FromBody] CreateMultipleChoiceSubmissionRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new CreateMultipleChoiceSubmission(
            request.AssignmentId,
            request.Details,
            userId
        );

        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetSubmission", new { id });
    }
}