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
        var data = await _assignments
            .Where(a => a.Id == query.Id)
            .Include(a => a.Activities)
            .Include(a => a.StudyGroup)
            .ThenInclude(g => g.Members)
            .Select(a => new
            {
                a.Id,
                a.Name,
                a.Description,
                a.AuthorId,
                a.StudyGroupId,
                a.DueDate,
                Activities = a.Activities
                    .Select(ac => new { ac.Id, ac.MaxScore, ac.Details, ac.Order })
                    .OrderBy(ac => ac.Order)
                    .ToList(),
                Submitted = a.Submissions.Any(s => s.StudentId == query.UserId),
                IsAuthor = (a.AuthorId == query.UserId),
                IsGroupMember = a.StudyGroup.OwnerId == query.UserId ||
                                a.StudyGroup.Members.Any(m => m.Id == query.UserId),
            })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (data is null)
            return null;

        if (!data.IsAuthor && !data.IsGroupMember)
            throw new UnauthorizedException(query.UserId);

        var assignmentDto = new AssignmentDto(
            data.Id,
            data.Name,
            data.Description,
            data.AuthorId,
            data.StudyGroupId,
            data.DueDate,
            data.Activities.Sum(ac => ac.MaxScore),
            data.Submitted,
            data.Activities
                .Select(ac => new ActivityDto(ac.Id, ac.MaxScore, ac.Details.ToDto()))
                .ToList()
        );

        return assignmentDto;
    }
}