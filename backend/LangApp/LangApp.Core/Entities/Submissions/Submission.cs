using LangApp.Core.Common;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Submissions;

public class Submission : AggregateRoot
{
    public Guid ExerciseId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTime SubmittedAt { get; private set; } = DateTime.Now;
    public Evaluation? EvaluationResult { get; private set; }

    internal Submission(Guid exerciseId, Guid studentId)
    {
        ExerciseId = exerciseId;
        StudentId = studentId;
    }

    public void Evaluate(Evaluation evaluationResult)
    {
        EvaluationResult = evaluationResult;
    }
}