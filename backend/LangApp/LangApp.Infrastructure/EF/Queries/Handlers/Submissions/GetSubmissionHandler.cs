using LangApp.Application.Assignments.Services.PolicyServices;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Submissions;
using LangApp.Infrastructure.EF.Queries.Handlers.Submissions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Submissions;

internal sealed class GetSubmissionHandler : IQueryHandler<GetSubmission, SubmissionDto>
{
    private readonly DbSet<SubmissionReadModel> _submissions;
    private readonly DbSet<AssignmentReadModel> _assignments;
    private readonly IAssignmentRestrictedAccessPolicyService _assignmentAccessPolicy;

    public GetSubmissionHandler(ReadDbContext context, IAssignmentRestrictedAccessPolicyService assignmentAccessPolicy)
    {
        _submissions = context.Submissions;
        _assignments = context.Assignments;
        _assignmentAccessPolicy = assignmentAccessPolicy;
    }

    public async Task<SubmissionDto?> HandleAsync(GetSubmission query)
    {
        var submission = await _submissions
            .Where(s => s.Id == query.Id)
            .Select(s => new SubmissionDto(
                s.Id,
                s.AssignmentId,
                s.StudentId,
                s.Type,
                s.Details.ToDto(),
                s.SubmittedAt,
                s.Status,
                s.Grade != null
                    ? new SubmissionGradeDto
                    {
                        ScorePercentage = s.Grade.ScorePercentage,
                        Feedback = s.Grade.Feedback
                    }
                    : null
            ))
            .AsNoTracking()
            .SingleOrDefaultAsync();


        if (submission is null)
        {
            return null;
        }

        // TODO wrong access logic here
        var groupId = await _assignments
            .Where(a => a.Id == submission.AssignmentId)
            .Select(a => a.GroupId)
            .SingleOrDefaultAsync();

        var isAllowed = await _assignmentAccessPolicy.IsSatisfiedBy(submission.AssignmentId, groupId, query.UserId);
        if (!isAllowed)
        {
            throw new UnauthorizedException(query.UserId, submission.Id, "Submission");
        }

        return submission;
    }
}
