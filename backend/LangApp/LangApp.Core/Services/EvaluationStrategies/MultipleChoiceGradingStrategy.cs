using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Core.Services.EvaluationStrategies;

public class MultipleChoiceGradingStrategy
    : IGradingStrategy<MultipleChoiceAssignmentDetails, MultipleChoiceSubmissionDetails>
{
    public SubmissionGrade Evaluate(MultipleChoiceAssignmentDetails assignment,
        MultipleChoiceSubmissionDetails submission)
    {
        if (submission.Answers.Count != assignment.Questions.Count)
        {
            throw new LangAppException("Invalid multiple choice answer count");
        }

        double correctOptions = assignment.Questions.Zip(submission.Answers)
            .Count(t => t.First.CorrectOptionIndex == t.Second.ChosenOptionIndex);

        Percentage percentage = new(correctOptions / assignment.Questions.Count);

        return new SubmissionGrade(percentage);
    }
}