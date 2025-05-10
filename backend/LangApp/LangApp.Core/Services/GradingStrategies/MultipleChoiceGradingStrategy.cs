using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Core.Services.GradingStrategies;

public class MultipleChoiceGradingStrategy
    : SynchronousGradingStrategy<MultipleChoiceActivityDetails>
{
    protected override SubmissionGrade ExecuteGrade(MultipleChoiceActivityDetails activity,
        SubmissionDetails submission,
        CancellationToken cancellationToken)
    {
        if (submission is not MultipleChoiceSubmissionDetails multipleChoiceSubmissionDetails)
        {
            throw new LangAppException(
                $"Provided submission {submission.GetType()} is not compatible with the assignment {activity.GetType()}");
        }

        if (multipleChoiceSubmissionDetails.Answers.Count != activity.Questions.Count)
        {
            throw new LangAppException("Invalid multiple choice answer count");
        }

        double correctOptions = activity.Questions.Zip(multipleChoiceSubmissionDetails.Answers)
            .Count(t => t.First.CorrectOptionIndex == t.Second.ChosenOptionIndex);

        Percentage percentage = new(correctOptions / activity.Questions.Count * 100);

        return new SubmissionGrade(percentage.Value);
    }
}
