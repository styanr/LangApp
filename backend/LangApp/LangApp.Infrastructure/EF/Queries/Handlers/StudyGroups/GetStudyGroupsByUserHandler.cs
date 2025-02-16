using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Dto;
using LangApp.Application.StudyGroups.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.StudyGroups;

internal sealed class GetStudyGroupsByUserHandler : IQueryHandler<GetStudyGroupsByUser, PagedResult<StudyGroupSlimDto>>
{
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetStudyGroupsByUserHandler(ReadDbContext context)
    {
        _groups = context.StudyGroups;
    }

    public async Task<PagedResult<StudyGroupSlimDto>?> HandleAsync(GetStudyGroupsByUser query)
    {
        var groups = await _groups
            .Include(g => g.Members)
            .Where(g => g.Members.Any(g => g.Id == query.UserId))
            .TakePage(query.PageNumber, query.PageSize)
            .AsNoTracking()
            .Select(g => new StudyGroupSlimDto(g.Id, g.Name, g.Language))
            .ToListAsync();

        var count = await _groups
            .Include(g => g.Members)
            .Where(g => g.Members.Any(g => g.Id == query.UserId))
            .CountAsync();

        return new PagedResult<StudyGroupSlimDto>(
            groups,
            count,
            query.PageNumber,
            query.PageSize
        );
    }
}