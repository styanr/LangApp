using LangApp.Core.Entities.Assignments;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;
using LangApp.Core.ValueObjects.Assignments.Question;
using LangApp.Core.ValueObjects.Assignments.Writing;

namespace LangApp.Core.Factories.Assignments;

public class ActivityFactory : IActivityFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public ActivityFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    private Activity CreateActivity(
        ActivityDetails details,
        int maxScore,
        ActivityType type)
    {
        var id = _keyGenerator.NewKey();
        return new Activity(id, details, maxScore, type);
    }

    public Activity Create(ActivityDetails details, int maxScore)
    {
        return details switch
        {
            MultipleChoiceActivityDetails multipleChoiceDetails =>
                CreateMultipleChoice(multipleChoiceDetails, maxScore),
            FillInTheBlankActivityDetails fillInTheBlankDetails =>
                CreateFillInTheBlank(fillInTheBlankDetails, maxScore),
            PronunciationActivityDetails pronunciationDetails => CreatePronunciation(pronunciationDetails, maxScore),
            QuestionActivityDetails questionDetails => CreateQuestion(questionDetails, maxScore),
            WritingActivityDetails writingDetails => CreateWriting(writingDetails, maxScore),
            _ => throw new ArgumentException($"Unsupported activity details type: {details.GetType().Name}",
                nameof(details))
        };
    }

    public Activity CreateMultipleChoice(
        MultipleChoiceActivityDetails activityDetails,
        int maxScore)
        => CreateActivity(
            activityDetails,
            maxScore,
            ActivityType.MultipleChoice);

    public Activity CreateFillInTheBlank(
        FillInTheBlankActivityDetails activityDetails,
        int maxScore)
        => CreateActivity(
            activityDetails,
            maxScore,
            ActivityType.FillInTheBlank);

    public Activity CreatePronunciation(
        PronunciationActivityDetails activityDetails,
        int maxScore)
        => CreateActivity(
            activityDetails,
            maxScore,
            ActivityType.Pronunciation);

    public Activity CreateQuestion(QuestionActivityDetails activityDetails, int maxScore)
        => CreateActivity(activityDetails, maxScore, ActivityType.Question);

    public Activity CreateWriting(WritingActivityDetails activityDetails, int maxScore)
        => CreateActivity(activityDetails, maxScore, ActivityType.Writing);
}
