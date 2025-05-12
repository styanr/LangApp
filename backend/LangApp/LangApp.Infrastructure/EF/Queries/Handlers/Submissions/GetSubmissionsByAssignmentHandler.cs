using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Models.Submissions;
using LangApp.Infrastructure.EF.Queries.Handlers.Submissions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Submissions;

internal class
    GetSubmissionsByAssignmentHandler : IQueryHandler<GetSubmissionsByAssignment, PagedResult<AssignmentSubmissionDto>>
{
    private readonly DbSet<AssignmentSubmissionReadModel> _submissions;
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly DbSet<StudyGroupReadModel> _groups;


    public GetSubmissionsByAssignmentHandler(ReadDbContext context)
    {
        _submissions = context.Submissions;
        _assignments = context.Assignments;
        _groups = context.StudyGroups;
    }


    public async Task<PagedResult<AssignmentSubmissionDto>?> HandleAsync(GetSubmissionsByAssignment query)
    {
        var assignment = _assignments.Include(a => a.StudyGroup).FirstOrDefault(a => a.Id == query.AssignmentId);

        if (assignment is null)
        {
            throw new AssignmentNotFound(query.AssignmentId);
        }

        if (assignment.StudyGroup.OwnerId != query.UserId)
        {
            throw new UnauthorizedException(query.UserId, query.AssignmentId, "Assignment");
        }

        var submissionEntities = _submissions
            .Where(s => s.AssignmentId == query.AssignmentId)
            .Include(s => s.ActivitySubmissions)
            .ThenInclude(asub => asub.Grade)
            .Include(s => s.ActivitySubmissions)
            .TakePage(query.PageNumber, query.PageSize)
            .ToList();

        var submissions = submissionEntities
            .Select(s => new AssignmentSubmissionDto(
                s.Id,
                s.AssignmentId,
                s.StudentId,
                s.ActivitySubmissions.Select(asub =>
                {
                    var gradeDto = asub.Grade != null
                        ? new SubmissionGradeDto
                        {
                            ScorePercentage = asub.Grade.ScorePercentage,
                            Feedback = asub.Grade.Feedback ?? ""
                        }
                        : null;

                    return new ActivitySubmissionDto(
                        asub.Id,
                        asub.ActivityId,
                        asub.Type,
                        asub.Details.ToDto(),
                        asub.Status,
                        gradeDto,
                        asub.FailureReason
                    );
                }).ToList()
            ))
            .ToList();
        var totalCount = await _submissions.Where(s => s.AssignmentId == query.AssignmentId).CountAsync();

        return new PagedResult<AssignmentSubmissionDto>(submissions, totalCount, query.PageNumber, query.PageSize);
    }
}
