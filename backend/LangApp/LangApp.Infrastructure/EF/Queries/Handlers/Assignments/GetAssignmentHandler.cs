using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Queries;
using LangApp.Application.Assignments.Services.PolicyServices;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Queries.Handlers.Assignments.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Assignments;

internal sealed class GetAssignmentHandler : IQueryHandler<GetAssignment, AssignmentDto>
{
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly IAssignmentFullAccessPolicyService _policy;

    public GetAssignmentHandler(ReadDbContext context, IAssignmentFullAccessPolicyService policy)
    {
        _policy = policy;
        _assignments = context.Assignments;
    }

    public async Task<AssignmentDto?> HandleAsync(GetAssignment query)
    {
        var assignment = await _assignments
            .Where(a => a.Id == query.Id)
            .Select(a => new AssignmentDto(
                a.Id,
                a.AuthorId,
                a.GroupId,
                a.DueTime,
                a.MaxScore,
                a.Type,
                a.Details.ToDto()
            ))
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (assignment is null)
        {
            return null;
        }

        var isAllowed = await _policy.IsSatisfiedBy(query.Id, query.UserId);
        if (!isAllowed)
        {
            throw new UnauthorizedException(query.UserId);
        }

        return assignment;
    }
}
