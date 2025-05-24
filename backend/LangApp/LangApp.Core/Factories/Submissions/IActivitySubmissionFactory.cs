using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;
using LangApp.Core.ValueObjects.Submissions.Question;
using LangApp.Core.ValueObjects.Submissions.Writing;

namespace LangApp.Core.Factories.Submissions;

public interface IActivitySubmissionFactory
{
    ActivitySubmission Create(Guid activityId, SubmissionDetails details);

    ActivitySubmission CreateMultipleChoice(Guid activityId,
        MultipleChoiceSubmissionDetails details);

    ActivitySubmission CreatePronunciation(Guid activityId, PronunciationSubmissionDetails details);

    ActivitySubmission CreateQuestion(Guid activityId, QuestionSubmissionDetails details);
    ActivitySubmission CreateWriting(Guid activityId, WritingSubmissionDetails details);
}
