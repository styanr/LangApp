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
        group.MapPost("/", Create).WithName("CreateAssignment");

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
        HttpContext context,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        var userId = context.User.GetUserId();
        var query = new GetAssignmentsByGroup(request.GroupId, userId)
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
            request.DueDate,
            request.Activities
        );

        var id = await dispatcher.DispatchWithResultAsync(command);
        return TypedResults.CreatedAtRoute("GetAssignment", new { id });
    }
}
