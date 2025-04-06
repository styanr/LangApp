using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Queries;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Queries.Handlers.Assignments.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Assignments;

internal class GetAssignmentHandler : IQueryHandler<GetAssignment, AssignmentDto>
{
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetAssignmentHandler(ReadDbContext context)
    {
        _assignments = context.Assignments;
        _groups = context.StudyGroups;
    }

    public Task<AssignmentDto?> HandleAsync(GetAssignment query)
    {
        var assignment = _assignments
            .Where(a => a.Id == query.Id)
            .Select(a => new AssignmentDto(
                a.Id,
                a.AuthorId,
                a.GroupId,
                a.DueTime,
                a.MaxScore,
                a.Type,
                a.Details.ToDto()
            ))
            .AsNoTracking()
            .SingleOrDefaultAsync();

        return assignment;
    }
}