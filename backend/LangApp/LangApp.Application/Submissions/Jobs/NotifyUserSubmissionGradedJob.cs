using LangApp.Application.Assignments.Jobs;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Application.StudyGroups.Jobs;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.Submissions.Jobs;

public record NotifyUserSubmissionGradedJobData(Guid UserId, Guid SubmissionId, double Score)
    : IJobData;

public class NotifyUserSubmissionGradedJob : IJob<NotifyUserSubmissionGradedJobData>
{
    private readonly IAssignmentSubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IStudyGroupRepository _studyGroupRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotifyUserAddedToGroupJob> _logger;

    public NotifyUserSubmissionGradedJob(IApplicationUserRepository userRepository, IEmailService emailService,
        ILogger<NotifyUserAddedToGroupJob> logger, IAssignmentSubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository, IStudyGroupRepository studyGroupRepository)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _studyGroupRepository = studyGroupRepository;
    }

    public async Task ExecuteAsync(NotifyUserSubmissionGradedJobData jobData)
    {
        var (userId, submissionId, grade) = jobData;
        _logger.LogInformation("Notifying user {UserId} about graded submission {SubmissionId} with score {Grade}",
            userId, submissionId, grade);

        var submission = await _submissionRepository.GetAsync(submissionId);

        if (submission is null)
        {
            _logger.LogError("Submission with id {SubmissionId} not found", submissionId);
            return;
        }

        var assignment = await _assignmentRepository.GetAsync(submission.AssignmentId);
        if (assignment is null)
        {
            _logger.LogError("Assignment with id {AssignmentId} not found", submission.AssignmentId);
            return;
        }

        var studyGroup = await _studyGroupRepository.GetAsync(assignment.StudyGroupId);
        if (studyGroup is null)
        {
            _logger.LogError("Study group with id {StudyGroupId} not found", assignment.StudyGroupId);
            return;
        }

        var user = await _userRepository.GetAsync(userId);

        if (user is null)
        {
            _logger.LogError("User with id {UserId} not found", userId);
            return;
        }

        await _emailService.SendEmailAsync(user.Email, $"Graded Submission \"{assignment.Name}\"",
            $"{user.FullName.FirstName}, your submission for the assignment \"{assignment.Name}\" in the study group \"{studyGroup.Name}\" has been graded. " +
            $"<br />Your score is {grade}. " +
            $"<br />You can view your submission and feedback in the study group portal.");
    }
}