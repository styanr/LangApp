using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Submissions;
using LangApp.Infrastructure.EF.Queries.Handlers.Assignments.Extensions;
using LangApp.Infrastructure.EF.Queries.Handlers.Submissions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Submissions;

internal class
    GetSubmissionsByUserGroupHandler : IQueryHandler<GetSubmissionsByUserGroup, PagedResult<UserGroupSubmissionDto>>
{
    private readonly DbSet<AssignmentSubmissionReadModel> _submissions;

    public GetSubmissionsByUserGroupHandler(ReadDbContext context)
    {
        _submissions = context.Submissions;
    }

    public async Task<PagedResult<UserGroupSubmissionDto>?> HandleAsync(GetSubmissionsByUserGroup query)
    {
        var submissionsQuery = _submissions
            .Where(s => s.Assignment.StudyGroupId == query.GroupId && s.StudentId == query.UserId)
            .OrderByDescending(s => s.SubmittedAt);

        var totalCount = await submissionsQuery.CountAsync();

        var submissionsList = await submissionsQuery
            .Include(s => s.Assignment)
                .ThenInclude(a => a.Activities)
            .Include(s => s.ActivitySubmissions)
                .ThenInclude(acs => acs.Grade)
            .TakePage(query.PageNumber, query.PageSize).ToListAsync();
        var submissions = submissionsList.Select(s => new UserGroupSubmissionDto(
            s.Assignment.ToDto(true, false),
            new AssignmentSubmissionDto(
                s.Id,
                s.AssignmentId,
                s.StudentId,
                s.SubmittedAt,
                s.Status,
                s.Score,
                s.ActivitySubmissions.Select(asub => new ActivitySubmissionDto(
                    asub.Id,
                    asub.ActivityId,
                    asub.Details.ToDto(),
                    asub.Status,
                    asub.Grade != null
                        ? new SubmissionGradeDto
                        {
                            ScorePercentage = asub.Grade.ScorePercentage,
                            Feedback = asub.Grade?.Feedback ?? ""
                        }
                        : null,
                    asub.FailureReason
                )).ToList()
            )));

        return new PagedResult<UserGroupSubmissionDto>
        (
            submissions.ToList(),
            totalCount,
            query.PageNumber,
            query.PageSize
        );
    }
}