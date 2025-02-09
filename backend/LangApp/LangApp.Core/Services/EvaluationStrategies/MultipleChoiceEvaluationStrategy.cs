using LangApp.Core.Entities.Exercises;
using LangApp.Core.Entities.Submissions;
using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Services.EvaluationStrategies;

public class MultipleChoiceEvaluationStrategy : IEvaluationStrategy<MultipleChoiceExercise, MultipleChoiceSubmission>
{
    public Evaluation Evaluate(MultipleChoiceExercise exercise, MultipleChoiceSubmission submission)
    {
        if (submission.SelectedOptions.Count != exercise.Questions.Count)
        {
            throw new LangAppException("Invalid multiple choice answer count");
        }

        double correctOptions = exercise.Questions.Zip(submission.SelectedOptions)
            .Count(t => t.First.CorrectOption == t.Second);

        Percentage percentage = new(correctOptions / exercise.Questions.Count);

        return new Evaluation(percentage);
    }
}