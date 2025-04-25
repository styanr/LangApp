using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Core.Factories.Submissions;

public interface ISubmissionFactory
{
    Submission CreateMultipleChoice(Guid assignmentId, Guid studentId, MultipleChoiceSubmissionDetails details);
}