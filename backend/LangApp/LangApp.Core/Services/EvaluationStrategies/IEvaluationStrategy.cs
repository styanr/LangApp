using LangApp.Core.Entities.Exercises;
using LangApp.Core.Entities.Submissions;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Services.EvaluationStrategies;

public interface IEvaluationStrategy<TExercise, TSubmission> where TExercise : Exercise where TSubmission : Submission
{
    Evaluation Evaluate(TExercise exercise, TSubmission submission);
}