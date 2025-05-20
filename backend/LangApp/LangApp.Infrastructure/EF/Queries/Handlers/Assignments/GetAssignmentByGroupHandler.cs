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
        bool canAccess = await _groups
            .AsNoTracking()
            .Where(g => g.Id == query.GroupId)
            .AnyAsync(g => g.OwnerId == query.UserId || g.Members.Any(m => m.Id == query.UserId));

        if (!canAccess)
            throw new UnauthorizedException(query.UserId, query.GroupId, "StudyGroup");

        var baseQuery = _assignments
            .Where(a => a.StudyGroupId == query.GroupId)
            .AsNoTracking();

        if (!query.ShowSubmitted)
            baseQuery = baseQuery.Where(a => a.Submissions.All(s => s.StudentId != query.UserId));

        int totalCount = await baseQuery.CountAsync();

        var assignments = await baseQuery
            .OrderByDescending(a => a.DueDate)
            .TakePage(query.PageNumber, query.PageSize)
            .Select(a => new AssignmentSlimDto(
                a.Id,
                a.Name,
                a.Description,
                a.AuthorId,
                a.StudyGroupId,
                a.DueDate,
                a.Activities.Sum(ac => ac.MaxScore),
                a.Submissions.Any(s => s.StudentId == query.UserId),
                a.Activities.Count
            ))
            .ToListAsync();

        return new PagedResult<AssignmentSlimDto>(
            assignments,
            totalCount,
            query.PageNumber,
            query.PageSize
        );
    }
}