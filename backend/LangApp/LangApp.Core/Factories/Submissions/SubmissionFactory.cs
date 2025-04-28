using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Core.Factories.Submissions;

public class SubmissionFactory : ISubmissionFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public SubmissionFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    private Submission Create(Guid assignmentId, Guid studentId, SubmissionDetails details, AssignmentType type)
    {
        var id = _keyGenerator.NewKey();

        return Submission.Create(assignmentId, studentId, details, type, id);
    }

    public Submission CreateMultipleChoice(Guid assignmentId, Guid studentId, MultipleChoiceSubmissionDetails details)
    {
        return Create(assignmentId, studentId, details, AssignmentType.MultipleChoice);
    }

    public Submission CreatePronunciation(Guid assignmentId, Guid studentId, PronunciationSubmissionDetails details)
    {
        return Create(assignmentId, studentId, details, AssignmentType.Pronunciation);
    }
}
