using LangApp.Core.Entities.Assignments;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;
using LangApp.Core.ValueObjects.Assignments.Question;
using LangApp.Core.ValueObjects.Assignments.Writing;

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

    Activity CreateQuestion(
        QuestionActivityDetails activityDetails,
        int maxScore
    );

    Activity CreateWriting(
        WritingActivityDetails activityDetails,
        int maxScore
    );
}
