using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Dto;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Application.StudyGroups.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.StudyGroups;

internal sealed class GetStudyGroupHandler : IQueryHandler<GetStudyGroup, StudyGroupDto>
{
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetStudyGroupHandler(ReadDbContext context)
    {
        _groups = context.StudyGroups;
    }

    public async Task<StudyGroupDto?> HandleAsync(GetStudyGroup query)
    {
        var group = await _groups
            .Include(g => g.Owner)
            .Include(g => g.Members)
            .Where(g => g.Id == query.Id)
            .Select(g => new StudyGroupDto
            (
                g.Id,
                g.Name,
                g.Language,
                new StudyGroupOwnerDto(
                    g.Owner.Id,
                    g.Owner.FullName.ToDto(),
                    g.Owner.PictureUrl
                ),
                g.Members.Select(m =>
                    new StudyGroupMemberDto(
                        m.Id,
                        m.FullName.ToDto(),
                        m.PictureUrl
                    )
                )
            )).AsNoTracking().SingleOrDefaultAsync();

        return group ?? throw new StudyGroupNotFound(query.Id);
    }
}