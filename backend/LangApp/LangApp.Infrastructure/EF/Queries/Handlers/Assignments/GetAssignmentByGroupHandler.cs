using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Queries;
using LangApp.Application.Common;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Queries.Handlers.Assignments.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Assignments;

internal sealed class
    GetAssignmentForGroupHandler : IQueryHandler<GetAssignmentsByGroup, PagedResult<AssignmentSlimDto>>
{
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetAssignmentForGroupHandler(ReadDbContext context)
    {
        _assignments = context.Assignments;
        _groups = context.StudyGroups;
    }

    public async Task<PagedResult<AssignmentSlimDto>?> HandleAsync(GetAssignmentsByGroup query)
    {
        var group = await _groups
            .Include(g => g.Members)
            .AsNoTracking()
            .SingleOrDefaultAsync(g => g.Id == query.GroupId);

        if (group is null)
        {
            return null;
        }

        bool canAccess = group.OwnerId == query.UserId || group.Members.Any(m => m.Id == query.UserId);

        if (!canAccess)
        {
            throw new UnauthorizedException(query.UserId, query.GroupId, "StudyGroup");
        }

        // Get all assignments for the group
        var baseQuery = _assignments
            .Where(a => a.StudyGroupId == query.GroupId)
            .Include(a => a.Activities.OrderBy(ac => ac.Order))
            .Include(a => a.Submissions)
            .AsNoTracking();

        // If showSubmitted is false or not set, filter out assignments already submitted by the user
        if (!query.ShowSubmitted)
        {
            baseQuery = baseQuery.Where(a => a.Submissions.All(s => s.StudentId != query.UserId));
        }

        var totalCount = await baseQuery.CountAsync();

        var assignments = await baseQuery
            .TakePage(query.PageNumber, query.PageSize)
            .Select(a => a.ToSlimDto())
            .ToListAsync();

        return new PagedResult<AssignmentSlimDto>(
            assignments,
            totalCount,
            query.PageNumber,
            query.PageSize
        );
    }
}