using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Queries;
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
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetAssignmentHandler(ReadDbContext context)
    {
        _assignments = context.Assignments;
        _groups = context.StudyGroups;
    }

    public async Task<AssignmentDto?> HandleAsync(GetAssignment query)
    {
        var assignment = await _assignments
            .Where(a => a.Id == query.Id)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (assignment is null)
        {
            return null;
        }

        // Direct permission check
        bool isAuthor = assignment.AuthorId == query.UserId;
        bool isGroupMember = false;

        if (!isAuthor)
        {
            var group = await _groups
                .Include(g => g.Members)
                .AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == assignment.GroupId);

            if (group is not null)
            {
                isGroupMember = group.OwnerId == query.UserId ||
                                group.Members.Any(m => m.Id == query.UserId);
            }
        }

        if (!isAuthor && !isGroupMember)
        {
            throw new UnauthorizedException(query.UserId);
        }

        return new AssignmentDto(
            assignment.Id,
            assignment.AuthorId,
            assignment.GroupId,
            assignment.DueTime,
            assignment.MaxScore,
            assignment.Type,
            assignment.Details.ToDto(!isAuthor)
        );
    }
}
