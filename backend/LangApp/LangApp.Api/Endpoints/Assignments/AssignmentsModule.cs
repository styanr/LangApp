using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.Assignments.Models;
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

        group.MapGet("/{id:guid}", Get).WithName("GetAssignment");
        group.MapGet("/{id:guid}/stats", GetStats).WithName("GetAssignmentStats");
        group.MapPost("/", Create).WithName("CreateAssignment");

        app.MapVersionedGroup("groups").WithTags("Assignments").MapGet("/{groupId:guid}/assignments", GetByGroup)
            .WithName("GetAssignmentsByGroup");

        app.MapVersionedGroup("users").WithTags("Assignments").MapGet("/me/assignments", GetByUser)
            .WithName("GetAssignmentsByUser");
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

    private async Task<Results<Ok<AssignmentSubmissionsStatisticsDto>, NotFound>> GetStats(
        [AsParameters] GetAssignmentRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetAssignmnentSubmissionsStatistics(request.Id, userId);
        var stats = await dispatcher.QueryAsync(query);
        return ApplicationTypedResults.OkOrNotFound(stats);
    }


    private async Task<Results<Ok<PagedResult<AssignmentSlimDto>>, NotFound>> GetByGroup(
        [AsParameters] GetAssignmentByGroupRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        var userId = context.User.GetUserId();
        var query = new GetAssignmentsByGroup(request.GroupId, userId, request.ShowSubmitted)
        {
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
        };
        var assignment = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(assignment);
    }

    private async Task<Results<Ok<PagedResult<AssignmentByUserSlimDto>>, NotFound>> GetByUser(
        [FromQuery] bool showSubmitted,
        [FromQuery] bool showOverdue,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        var userId = context.User.GetUserId();
        var query = new GetAssignmentsByUser(userId, showSubmitted, showOverdue)
        {
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
        };
        var assignment = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(assignment);
    }

    private async Task<CreatedAtRoute> Create(
        [FromBody] CreateAssignmentRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();

        var command = new CreateAssignment(
            request.Name,
            request.Description,
            userId,
            request.GroupId,
            request.DueDate.UtcDateTime,
            request.Activities
        );

        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetAssignment", new { id });
    }
}