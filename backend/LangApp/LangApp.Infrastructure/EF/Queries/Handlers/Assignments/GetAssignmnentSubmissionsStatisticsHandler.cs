using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Queries;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Submissions.Dto;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Submissions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Assignments;

internal class
    GetAssignmnentSubmissionsStatisticsHandler : IQueryHandler<GetAssignmnentSubmissionsStatistics,
    AssignmentSubmissionsStatisticsDto>
{
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly DbSet<AssignmentSubmissionReadModel> _submissions;

    public GetAssignmnentSubmissionsStatisticsHandler(ReadDbContext context)
    {
        _assignments = context.Assignments;
        _submissions = context.Submissions;
    }

    public async Task<AssignmentSubmissionsStatisticsDto?> HandleAsync(GetAssignmnentSubmissionsStatistics query)
    {
        var assignment = await _assignments
            .Include(a => a.StudyGroup)
            .ThenInclude(g => g.Members)
            .FirstOrDefaultAsync(a => a.Id == query.AssignmentId);

        if (assignment is null)
        {
            return null;
        }

        if (assignment.StudyGroup.OwnerId != query.UserId)
        {
            throw new UnauthorizedException(query.UserId, assignment.StudyGroupId, "StudyGroup");
        }

        var submissions = _submissions
            .Where(s => s.AssignmentId == assignment.Id)
            .Include(s => s.Student);

        var submissionInfos = await submissions
            .Select(s => new
            {
                s.Id,
                s.StudentId,
                s.Student.Username,
                s.Student.PictureUrl,
                s.Status
            })
            .ToListAsync();

        var totalCount = submissionInfos.Count;
        var pendingCount = submissionInfos.Count(x => x.Status == GradeStatus.Pending);
        var failedCount = submissionInfos.Count(x => x.Status == GradeStatus.Failed);
        var completedCount = submissionInfos.Count(x => x.Status == GradeStatus.Completed);
        var needsReviewCount = submissionInfos.Count(x => x.Status == GradeStatus.NeedsReview);

        return new AssignmentSubmissionsStatisticsDto(
            assignment.Id,
            totalCount,
            pendingCount,
            failedCount,
            completedCount,
            needsReviewCount,
            submissionInfos.Select(x => new AssignmentSubmissionInfoDto(
                x.Id,
                x.StudentId,
                x.Username,
                x.PictureUrl)).ToList()
        );
    }
}