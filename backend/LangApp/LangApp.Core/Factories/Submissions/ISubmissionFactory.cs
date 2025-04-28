using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Core.Factories.Submissions;

public interface ISubmissionFactory
{
    Submission CreateMultipleChoice(Guid assignmentId, Guid studentId, MultipleChoiceSubmissionDetails details);
    Submission CreatePronunciation(Guid assignmentId, Guid studentId, PronunciationSubmissionDetails details);
}
