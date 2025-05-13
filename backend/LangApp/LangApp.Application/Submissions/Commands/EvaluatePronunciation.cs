using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Application.Submissions.Dto;
using LangApp.Core.Exceptions;
using LangApp.Core.Factories.Assignments;
using LangApp.Core.Repositories;
using LangApp.Core.Services.GradingStrategies;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Application.Submissions.Commands;

public record EvaluatePronunciation(
    string RecordingUrl,
    Guid AssignmentId,
    Guid ActivityId,
    Guid UserId
) : ICommand<SubmissionGradeDto>;

public class EvaluatePronunciationHandler : ICommandHandler<EvaluatePronunciation, SubmissionGradeDto>
{
    private readonly IGradingStrategy<PronunciationActivityDetails> _gradingStrategy;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IStudyGroupRepository _studyGroupRepository;
    private readonly IActivityFactory _activityFactory;


    public EvaluatePronunciationHandler(IGradingStrategy<PronunciationActivityDetails> gradingStrategy,
        IAssignmentRepository assignmentRepository, IStudyGroupRepository studyGroupRepository,
        IActivityFactory activityFactory)
    {
        _gradingStrategy = gradingStrategy;
        _assignmentRepository = assignmentRepository;
        _studyGroupRepository = studyGroupRepository;
        _activityFactory = activityFactory;
    }

    public async Task<SubmissionGradeDto> HandleAsync(EvaluatePronunciation command,
        CancellationToken cancellationToken)
    {
        var (recordingUrl, assignmentId, activityId, userId) = command;
        var assignment = await _assignmentRepository.GetAsync(assignmentId) ??
                         throw new AssignmentNotFound(assignmentId);
        var studyGroup = await _studyGroupRepository.GetAsync(assignment.StudyGroupId) ??
                         throw new StudyGroupNotFound(assignment.StudyGroupId);

        var activity = assignment.Activities.FirstOrDefault(a => a.Id == activityId) ??
                       throw new ActivityNotFound(activityId);

        if (activity.Details is not PronunciationActivityDetails activityDetails)
        {
            throw new LangAppException("Provided activity is not a pronunciation activity");
        }

        var submissionDetails = new PronunciationSubmissionDetails(recordingUrl);

        var result = await _gradingStrategy.GradeAsync(activityDetails, submissionDetails, cancellationToken);

        return new SubmissionGradeDto
        {
            ScorePercentage = result.ScorePercentage.Value,
            Feedback = result.Feedback ?? string.Empty
        };
    }
}
