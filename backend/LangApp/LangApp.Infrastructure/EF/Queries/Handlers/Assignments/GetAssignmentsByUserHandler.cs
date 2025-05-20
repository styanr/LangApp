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
        var userExists = await _users.AnyAsync(u => u.Id == query.UserId);
        if (!userExists)
            throw new UserNotFoundException(query.UserId);

        var assignmentsQuery = _assignments
            .AsNoTracking()
            .Where(a => a.StudyGroup.Members.Any(u => u.Id == query.UserId));

        // If ShowSubmitted is false or not set, filter out assignments already submitted by the user
        if (!query.ShowSubmitted)
        {
            assignmentsQuery = assignmentsQuery.Where(a => a.Submissions.All(s => s.StudentId != query.UserId));
        }

        var totalCount = await assignmentsQuery.CountAsync();

        var pagedAssignments = await assignmentsQuery
            .Include(a => a.Activities.OrderBy(ac => ac.Order))
            .Include(a => a.Submissions)
            .Where(a => a.DueDate >= DateTime.UtcNow)
            .OrderByDescending(a => a.DueDate)
            .TakePage(query.PageNumber, query.PageSize)
            .Select(a => a.ToSlimDto())
            .ToListAsync();

        return new PagedResult<AssignmentSlimDto>(pagedAssignments, totalCount, query.PageNumber, query.PageSize);
    }
}