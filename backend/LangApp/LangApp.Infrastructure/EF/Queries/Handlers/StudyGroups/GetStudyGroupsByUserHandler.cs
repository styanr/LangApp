using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Dto;
using LangApp.Application.StudyGroups.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Models.Users;
using LangApp.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.StudyGroups;

internal sealed class GetStudyGroupsByUserHandler : IQueryHandler<GetStudyGroupsByUser, PagedResult<StudyGroupSlimDto>>
{
    private readonly DbSet<StudyGroupReadModel> _groups;
    private readonly DbSet<UserReadModel> _users;

    public GetStudyGroupsByUserHandler(ReadDbContext context)
    {
        _groups = context.StudyGroups;
        _users = context.Users;
    }

    public async Task<PagedResult<StudyGroupSlimDto>?> HandleAsync(GetStudyGroupsByUser query)
    {
        var user = await _users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == query.UserId);

        if (user == null)
        {
            return new PagedResult<StudyGroupSlimDto>(
                new List<StudyGroupSlimDto>(),
                0,
                query.PageNumber,
                query.PageSize
            );
        }

        var groupsQuery = _groups
            .Include(g => g.Members)
            .AsNoTracking();

        groupsQuery = user.Role == UserRole.Teacher ? groupsQuery.Where(g => g.OwnerId == query.UserId) :
            groupsQuery.Where(g => g.Members.Any(m => m.Id == query.UserId));

        var groups = await groupsQuery
            .TakePage(query.PageNumber, query.PageSize)
            .Select(g => new StudyGroupSlimDto(g.Id, g.Name, g.Language))
            .ToListAsync();

        var count = await groupsQuery.CountAsync();

        return new PagedResult<StudyGroupSlimDto>(
            groups,
            count,
            query.PageNumber,
            query.PageSize
        );
    }
}
