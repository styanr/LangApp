using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Queries;
using LangApp.Application.Auth.Exceptions;
using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Models.Users;
using LangApp.Infrastructure.EF.Queries.Handlers.Assignments.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Assignments;

internal class GetAssignmentsByUserHandler : IQueryHandler<GetAssignmentsByUser, PagedResult<AssignmentSlimDto>>
{
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly DbSet<UserReadModel> _users;
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetAssignmentsByUserHandler(ReadDbContext context)
    {
        _assignments = context.Assignments;
        _users = context.Users;
        _groups = context.StudyGroups;
    }

    public async Task<PagedResult<AssignmentSlimDto>?> HandleAsync(GetAssignmentsByUser query)
    {
        if (!await _users.AnyAsync(u => u.Id == query.UserId))
            throw new UserNotFoundException(query.UserId);

        var baseQuery = _assignments
            .AsNoTracking()
            .Where(a =>
                (a.StudyGroup.OwnerId == query.UserId || a.StudyGroup.Members.Any(m => m.Id == query.UserId))
            );

        if (!query.ShowOverdue)
            baseQuery = baseQuery.Where(a => a.DueDate >= DateTime.UtcNow);

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