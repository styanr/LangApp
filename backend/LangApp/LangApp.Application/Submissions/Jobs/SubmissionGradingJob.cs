using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Strategies;
using LangApp.Application.Submissions.Exceptions;
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
        IAssignmentRepository assignmentRepository,
        ILogger<SubmissionGradingJob> logger, IGradingStrategyDispatcher dispatcher)
    {
        _assignmentSubmissionRepository = assignmentSubmissionRepository;
        _assignmentRepository = assignmentRepository;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    public async Task ExecuteAsync(SubmissionGradingJobData data)
    {
        var submissionId = data.SubmissionId;
        _logger.LogInformation("RECEIVED SubmissionCreated. ID={SubmissionId}", submissionId);

        try
        {
            var submission = await _assignmentSubmissionRepository.GetAsync(submissionId)
                             ?? throw new SubmissionNotFound(submissionId);

            var assignment = await _assignmentRepository.GetAsync(submission.AssignmentId)
                             ?? throw new AssignmentNotFound(submission.AssignmentId);

            foreach (var activitySubmission in submission.ActivitySubmissions)
            {
                try
                {
                    var activity = assignment.Activities.FirstOrDefault(ac => ac.Id == activitySubmission.Id)
                                   ?? throw new ActivityNotFound(activitySubmission.Id);

                    var grade = await _dispatcher.Grade(activity.Details, activitySubmission.Details);

                    submission.GradeActivitySubmission(activitySubmission, grade);

                    var scoreContribution = grade.ScorePercentage * activity.MaxScore / 100;
                    submission.UpdateScore(submission.Score + scoreContribution);

                    _logger.LogInformation("Grading successful: {@Grade}", grade);
                }
                catch (Exception gradingEx)
                {
                    _logger.LogError(gradingEx, "Error grading activity submission ID={ActivitySubmissionId}",
                        activitySubmission.Id);
                    submission.FailActivitySubmission(activitySubmission);
                }
            }

            await _assignmentSubmissionRepository.UpdateAsync(submission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encountered during grading of submission ID={SubmissionId}", submissionId);
        }
    }
}
