using LangApp.Core.Entities.Assignments;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;

namespace LangApp.Core.Factories.Assignments;

public interface IActivityFactory
{
    Activity Create(ActivityDetails details, int maxScore);

    Activity CreateMultipleChoice(
        MultipleChoiceActivityDetails activityDetails,
        int maxScore);


    Activity CreateFillInTheBlank(
        FillInTheBlankActivityDetails activityDetails,
        int maxScore);

    Activity CreatePronunciation(
        PronunciationActivityDetails activityDetails,
        int maxScore);
}
