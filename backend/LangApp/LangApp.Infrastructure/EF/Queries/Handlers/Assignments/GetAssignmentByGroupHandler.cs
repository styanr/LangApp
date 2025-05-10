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

internal sealed class GetAssignmentForGroupHandler : IQueryHandler<GetAssignmentsByGroup, PagedResult<AssignmentDto>>
{
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetAssignmentForGroupHandler(ReadDbContext context)
    {
        _assignments = context.Assignments;
        _groups = context.StudyGroups;
    }

    public async Task<PagedResult<AssignmentDto>?> HandleAsync(GetAssignmentsByGroup query)
    {
        // Get the group with its members for permission checking
        var group = await _groups
            .Include(g => g.Members)
            .AsNoTracking()
            .SingleOrDefaultAsync(g => g.Id == query.GroupId);

        if (group is null)
        {
            return null;
        }

        // Direct permission check
        var canAccess = false;
        var canAccessFull = false;

        // Group owner can access all assignments in their group
        if (group.OwnerId == query.UserId)
        {
            canAccess = true;
            canAccessFull = true;
        }
        // Group members can access assignments in the group
        else if (group.Members.Any(m => m.Id == query.UserId))
        {
            canAccess = true;
        }

        if (!canAccess)
        {
            throw new UnauthorizedException(query.UserId, query.GroupId, "StudyGroup");
        }

        var totalCount = await _assignments.Where(a => a.StudyGroupId == query.GroupId).CountAsync();

        var assignments = await _assignments
            .Where(a => a.StudyGroupId == query.GroupId)
            .TakePage(query.PageNumber, query.PageSize)
            .AsNoTracking()
            .Select(a => new AssignmentDto(
                a.Id,
                a.AuthorId,
                a.StudyGroupId,
                a.DueDate,
                a.Activities.Select(ac =>
                    new ActivityDto(
                        ac.Id,
                        ac.MaxScore,
                        ac.Details.ToDto(!canAccessFull)
                    )
                ).ToList())).ToListAsync();

        return new PagedResult<AssignmentDto>(
            assignments,
            totalCount,
            query.PageNumber,
            query.PageSize
        );
    }
}
