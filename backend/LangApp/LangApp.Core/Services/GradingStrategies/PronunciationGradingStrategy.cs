using LangApp.Core.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Core.Services.GradingStrategies;

public class PronunciationGradingStrategy : IGradingStrategy<PronunciationActivityDetails>
{
    private readonly IPronunciationAssessmentService _assessmentService;

    public PronunciationGradingStrategy(IPronunciationAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }

    public async Task<SubmissionGrade> Grade(PronunciationActivityDetails activity, SubmissionDetails submission,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (submission is not PronunciationSubmissionDetails pronunciationSubmission)
        {
            throw new LangAppException(
                $"Provided submission {submission.GetType()} is not compatible with the assignment {activity.GetType()}");
        }

        var score = await _assessmentService.Assess(pronunciationSubmission.FileUri, activity.ReferenceText,
            activity.Language);

        // todo create a proper feedback message
        return new(score.Value, "good job!");
    }
}
