using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.FillInTheBlank;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;
using LangApp.Core.ValueObjects.Submissions.Question;
using LangApp.Core.ValueObjects.Submissions.Writing;

namespace LangApp.Core.Factories.Submissions;

public class ActivitySubmissionFactory : IActivitySubmissionFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public ActivitySubmissionFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    private ActivitySubmission Create(Guid activityId, SubmissionDetails details, ActivityType type)
    {
        var id = _keyGenerator.NewKey();

        return ActivitySubmission.Create(activityId, details, type, id);
    }

    public ActivitySubmission Create(Guid activityId, SubmissionDetails details)
    {
        return details switch
        {
            MultipleChoiceSubmissionDetails multipleChoiceDetails =>
                CreateMultipleChoice(activityId, multipleChoiceDetails),
            PronunciationSubmissionDetails pronunciationDetails =>
                CreatePronunciation(activityId, pronunciationDetails),
            QuestionSubmissionDetails questionDetails => CreateQuestion(activityId, questionDetails),
            WritingSubmissionDetails writingDetails => CreateWriting(activityId, writingDetails),
            FillInTheBlankSubmissionDetails fillInTheBlankDetails =>
                Create(activityId, fillInTheBlankDetails, ActivityType.FillInTheBlank),
            _ => throw new ArgumentException($"Unsupported activity details type: {details.GetType().Name}",
                nameof(details))
        };
    }

    public ActivitySubmission CreateMultipleChoice(Guid activityId,
        MultipleChoiceSubmissionDetails details)
    {
        return Create(activityId, details, ActivityType.MultipleChoice);
    }

    public ActivitySubmission CreatePronunciation(Guid activityId,
        PronunciationSubmissionDetails details)
    {
        return Create(activityId, details, ActivityType.Pronunciation);
    }

    public ActivitySubmission CreateQuestion(Guid activityId, QuestionSubmissionDetails details)
    {
        return Create(activityId, details, ActivityType.Question);
    }

    public ActivitySubmission CreateWriting(Guid activityId, WritingSubmissionDetails details)
    {
        return Create(activityId, details, ActivityType.Writing);
    }
}