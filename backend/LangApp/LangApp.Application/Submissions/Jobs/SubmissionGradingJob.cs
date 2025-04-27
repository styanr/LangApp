using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Strategies;
using LangApp.Application.Submissions.Exceptions;
using LangApp.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.Submissions.Jobs;

public record SubmissionGradingJobData(Guid SubmissionId) : IJobData;

public class SubmissionGradingJob : IJob<SubmissionGradingJobData>
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IGradingStrategyDispatcher _dispatcher;
    private readonly ILogger<SubmissionGradingJob> _logger;


    public SubmissionGradingJob(ISubmissionRepository submissionRepository, IAssignmentRepository assignmentRepository,
        ILogger<SubmissionGradingJob> logger, IGradingStrategyDispatcher dispatcher)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    public async Task ExecuteAsync(SubmissionGradingJobData data)
    {
        try
        {
            var id = data.SubmissionId;
            _logger.LogInformation("RECEIVED SubmissionCreated\nID={}", id);


            // TODO Submission deserialization wrong
            var submission = await _submissionRepository.GetAsync(id) ?? throw new SubmissionNotFound(id);
            var assignment = await _assignmentRepository.GetAsync(submission.AssignmentId) ??
                             throw new AssignmentNotFound(submission.AssignmentId);

            var grade = await _dispatcher.Grade(assignment.Details, submission.Details);
            submission.UpdateGrade(grade);
            await _submissionRepository.UpdateAsync(submission);

            _logger.LogInformation("Grading successful: {}", grade);
        }
        catch (Exception e)
        {
            _logger.LogError("Error encountered during grading process: {}", e);
        }
    }
}
