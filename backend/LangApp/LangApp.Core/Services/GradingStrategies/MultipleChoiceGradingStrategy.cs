using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Core.Services.GradingStrategies;

public class MultipleChoiceGradingStrategy
    : SynchronousGradingStrategy<MultipleChoiceActivityDetails>
{
    protected override SubmissionGrade ExecuteGrade(
        MultipleChoiceActivityDetails activity,
        SubmissionDetails submission,
        CancellationToken cancellationToken)
    {
        if (submission is not MultipleChoiceSubmissionDetails multipleChoiceSubmissionDetails)
        {
            throw new LangAppException(
                $"Provided submission {submission.GetType()} is not compatible with the assignment {activity.GetType()}");
        }

        if (activity.Questions.Count == 0)
        {
            throw new LangAppException("Grading failed: activity contains no questions.");
        }

        int correctOptions = 0;

        foreach (var answer in multipleChoiceSubmissionDetails.Answers)
        {
            if (answer.QuestionIndex < 0 || answer.QuestionIndex >= activity.Questions.Count)
            {
                continue;
            }

            var question = activity.Questions[answer.QuestionIndex];

            if (question.CorrectOptionIndex == answer.ChosenOptionIndex)
            {
                correctOptions++;
            }
        }

        double percentageValue = (double)correctOptions / activity.Questions.Count * 100;

        var percentage = new Percentage(percentageValue);
        return new SubmissionGrade(percentage);
    }
}
