using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Core.Factories.Submissions;

public interface IActivitySubmissionFactory
{
    ActivitySubmission Create(SubmissionDetails details);

    ActivitySubmission CreateMultipleChoice(
        MultipleChoiceSubmissionDetails details);

    ActivitySubmission CreatePronunciation(PronunciationSubmissionDetails details);
}
