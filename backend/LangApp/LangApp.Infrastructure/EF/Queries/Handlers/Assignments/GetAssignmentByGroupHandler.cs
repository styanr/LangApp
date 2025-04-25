using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Queries;
using LangApp.Application.Common;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Services.PolicyServices;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Queries.Handlers.Assignments.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Assignments;

internal sealed class GetAssignmentForGroupHandler : IQueryHandler<GetAssignmentByGroup, PagedResult<AssignmentDto>>
{
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly IStudyGroupAccessPolicyService _policy;

    public GetAssignmentForGroupHandler(ReadDbContext context, IStudyGroupAccessPolicyService policy)
    {
        _assignments = context.Assignments;
        _policy = policy;
    }

    public async Task<PagedResult<AssignmentDto>?> HandleAsync(GetAssignmentByGroup query)
    {
        var totalCount = await _assignments.Where(a => a.GroupId == query.GroupId).CountAsync();

        var canAccess = await _policy.IsSatisfiedBy(query.GroupId, query.UserId);

        if (!canAccess)
        {
            throw new UnauthorizedException(query.UserId, query.GroupId, "StudyGroup");
        }

        var assignments = await _assignments
            .Where(a => a.GroupId == query.GroupId)
            .TakePage(query.PageNumber, query.PageSize)
            .AsNoTracking()
            .Select(a => new AssignmentDto(
                a.Id,
                a.AuthorId,
                a.GroupId,
                a.DueTime,
                a.MaxScore,
                a.Type,
                a.Details.ToDto()
            )).ToListAsync();

        return new PagedResult<AssignmentDto>(
            assignments,
            totalCount,
            query.PageNumber,
            query.PageSize
        );
    }
}
