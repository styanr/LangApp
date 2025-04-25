using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Core.Factories.Submissions;

public class SubmissionFactory : ISubmissionFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public SubmissionFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    public Submission CreateMultipleChoice(Guid assignmentId, Guid studentId, MultipleChoiceSubmissionDetails details
    )
    {
        var id = _keyGenerator.NewKey();

        return new Submission(assignmentId, studentId, details, AssignmentType.MultipleChoice, id);
    }
}
