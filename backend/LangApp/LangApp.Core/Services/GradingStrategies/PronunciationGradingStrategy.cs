using LangApp.Core.Exceptions;
using LangApp.Core.Exceptions.Grading;
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

    public async Task<SubmissionGrade> GradeAsync(
        PronunciationActivityDetails activity,
        SubmissionDetails submission,
        CancellationToken cancellationToken = default)
    {
        if (submission is not PronunciationSubmissionDetails pronunciationSubmission)
        {
            throw new IncompatibleSubmissionTypeException(submission.GetType(), activity.GetType());
        }

        var score = await _assessmentService.Assess(
            pronunciationSubmission.FileUri,
            activity.ReferenceText,
            activity.Language
        );

        return score;
    }
}