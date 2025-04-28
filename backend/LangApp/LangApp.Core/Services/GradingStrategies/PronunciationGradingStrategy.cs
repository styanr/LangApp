using LangApp.Core.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Core.Services.GradingStrategies;

public class PronunciationGradingStrategy : IGradingStrategy<PronunciationAssignmentDetails>
{
    private readonly IPronunciationAssessmentService _assessmentService;

    public PronunciationGradingStrategy(IPronunciationAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }

    public async Task<SubmissionGrade> Grade(PronunciationAssignmentDetails assignment, SubmissionDetails submission,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (submission is not PronunciationSubmissionDetails pronunciationSubmission)
        {
            throw new LangAppException(
                $"Provided submission {submission.GetType()} is not compatible with the assignment {assignment.GetType()}");
        }

        var grade = await _assessmentService.Assess(pronunciationSubmission.FileUri, assignment.ReferenceText,
            assignment.Language);

        return grade;
    }
}
