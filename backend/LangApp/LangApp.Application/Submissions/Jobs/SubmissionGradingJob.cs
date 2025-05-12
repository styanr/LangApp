using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Strategies;
using LangApp.Application.Submissions.Exceptions;
using LangApp.Core.Entities.Assignments;
using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.Submissions.Jobs;

public record SubmissionGradingJobData(Guid SubmissionId) : IJobData;

public class SubmissionGradingJob : IJob<SubmissionGradingJobData>
{
    private readonly IAssignmentSubmissionRepository _assignmentSubmissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IGradingStrategyDispatcher _dispatcher;
    private readonly ILogger<SubmissionGradingJob> _logger;

    public SubmissionGradingJob(IAssignmentSubmissionRepository assignmentSubmissionRepository,
        IAssignmentRepository assignmentRepository, IGradingStrategyDispatcher dispatcher,
        ILogger<SubmissionGradingJob> logger)
    {
        _assignmentSubmissionRepository = assignmentSubmissionRepository ??
                                          throw new ArgumentNullException(nameof(assignmentSubmissionRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExecuteAsync(SubmissionGradingJobData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var submissionId = data.SubmissionId;
        _logger.LogInformation("RECEIVED SubmissionCreated. ID={SubmissionId}", submissionId);

        try
        {
            var (assignment, submission) = await LoadEntitiesAsync(submissionId);

            var activityLookup = assignment.Activities.ToDictionary(a => a.Id);

            var (gradedCount, failedCount) = await GradeActivitiesAsync(submission, activityLookup);

            _logger.LogInformation("Graded {GradedCount} activities. {FailedCount} failed.", gradedCount, failedCount);

            submission.RecalculateTotalScore(assignment.Activities);

            // Save changes
            await _assignmentSubmissionRepository.UpdateAsync(submission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error during grading of submission ID={SubmissionId}", submissionId);
            throw;
        }
    }

    private async Task<(Assignment, AssignmentSubmission)> LoadEntitiesAsync(Guid submissionId)
    {
        var submission = await _assignmentSubmissionRepository.GetAsync(submissionId)
                         ?? throw new SubmissionNotFound(submissionId);

        var assignment = await _assignmentRepository.GetAsync(submission.AssignmentId)
                         ?? throw new AssignmentNotFound(submission.AssignmentId);

        return (assignment, submission);
    }

    private async Task<(int GradedCount, int FailedCount)> GradeActivitiesAsync(
        AssignmentSubmission submission,
        Dictionary<Guid, Activity> activityLookup)
    {
        if (!submission.ActivitySubmissions.Any())
        {
            _logger.LogWarning("No activity submissions found for submission ID={SubmissionId}", submission.Id);
            return (0, 0);
        }

        int gradedCount = 0;
        int failedCount = 0;

        foreach (var submittedActivity in submission.ActivitySubmissions)
        {
            if (!activityLookup.TryGetValue(submittedActivity.ActivityId, out var activity))
            {
                _logger.LogWarning("Activity not found for submission. ActivityId={ActivityId}", submittedActivity.Id);
                submission.FailActivitySubmission(submittedActivity, reason: "Missing activity in assignment");
                failedCount++;
                continue;
            }

            if (!activity.Details.CanBeGradedAutomatically)
            {
                _logger.LogInformation("Skipping manual grading activity: {ActivityId}", activity.Id);
                continue;
            }

            var gradingResult = await GradeSingleActivityAsync(activity, submittedActivity);

            if (gradingResult.Success)
            {
                submission.GradeActivitySubmission(submittedActivity, gradingResult.Grade!);
                gradedCount++;
            }
            else
            {
                submission.FailActivitySubmission(submittedActivity, gradingResult.ErrorMessage);
                failedCount++;
            }
        }

        return (gradedCount, failedCount);
    }

    private async Task<(bool Success, SubmissionGrade? Grade, string? ErrorMessage)> GradeSingleActivityAsync(
        Activity activity,
        ActivitySubmission submittedActivity)
    {
        try
        {
            var grade = await _dispatcher.Grade(activity.Details, submittedActivity.Details);
            _logger.LogInformation("Graded activity: ActivityId={ActivityId}, Grade={@Grade}", activity.Id, grade);
            return (true, grade, null);
        }
        catch (LangAppException gradingEx)
        {
            _logger.LogError(gradingEx, "Grading error for ActivitySubmissionId={ActivitySubmissionId}",
                submittedActivity.Id);
            return (false, null, gradingEx.Message);
        }
        catch (Exception unexpected)
        {
            _logger.LogError(unexpected, "Unexpected error while grading ActivitySubmissionId={ActivitySubmissionId}",
                submittedActivity.Id);
            return (false, null, "Unexpected error");
        }
    }
}
