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

internal sealed class GetSubmissionHandler : IQueryHandler<GetSubmission, SubmissionDto>
{
    private readonly DbSet<SubmissionReadModel> _submissions;
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetSubmissionHandler(ReadDbContext context)
    {
        _submissions = context.Submissions;
        _assignments = context.Assignments;
        _groups = context.StudyGroups;
    }

    public async Task<SubmissionDto?> HandleAsync(GetSubmission query)
    {
        var submission = await _submissions
            .Where(s => s.Id == query.Id)
            .Include(s => s.Grade)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (submission is null)
        {
            return null;
        }

        // Get assignment and group info needed for permission check
        var assignment = await _assignments
            .Where(a => a.Id == submission.AssignmentId)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (assignment is null)
        {
            return null;
        }

        var group = await _groups
            .Include(g => g.Members)
            .Where(g => g.Id == assignment.GroupId)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (group is null)
        {
            return null;
        }

        // Direct permission check
        bool isAllowed = false;

        // Assignment author can access all submissions
        if (assignment.AuthorId == query.UserId)
        {
            isAllowed = true;
        }
        // Student can access their own submission
        else if (submission.StudentId == query.UserId)
        {
            isAllowed = true;
        }
        // Group owner can access all submissions in their group
        else if (group.OwnerId == query.UserId)
        {
            isAllowed = true;
        }

        if (!isAllowed)
        {
            throw new UnauthorizedException(query.UserId, submission.Id, "Submission");
        }

        return new SubmissionDto(
            submission.Id,
            submission.AssignmentId,
            submission.StudentId,
            submission.Type,
            submission.Details.ToDto(),
            submission.SubmittedAt,
            submission.Status,
            submission.Grade != null
                ? new SubmissionGradeDto
                {
                    ScorePercentage = submission.Grade.ScorePercentage,
                    Feedback = submission.Grade.Feedback
                }
                : null
        );
    }
}
