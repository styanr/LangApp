using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Core.Factories.Submissions;

public class ActivitySubmissionFactory : IActivitySubmissionFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public ActivitySubmissionFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    private ActivitySubmission Create(SubmissionDetails details, ActivityType type)
    {
        var id = _keyGenerator.NewKey();

        return ActivitySubmission.Create(details, type, id);
    }

    public ActivitySubmission Create(SubmissionDetails details)
    {
        return details switch
        {
            MultipleChoiceSubmissionDetails multipleChoiceDetails =>
                CreateMultipleChoice(multipleChoiceDetails),
            PronunciationSubmissionDetails pronunciationDetails => CreatePronunciation(pronunciationDetails),
            _ => throw new ArgumentException($"Unsupported activity details type: {details.GetType().Name}",
                nameof(details))
        };
    }

    public ActivitySubmission CreateMultipleChoice(
        MultipleChoiceSubmissionDetails details)
    {
        return Create(details, ActivityType.MultipleChoice);
    }

    public ActivitySubmission CreatePronunciation(
        PronunciationSubmissionDetails details)
    {
        return Create(details, ActivityType.Pronunciation);
    }
}
