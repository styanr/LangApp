using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects.Exercises;

namespace LangApp.Core.Entities.Exercises;

public class MultipleChoiceExercise : Exercise
{
    private readonly List<MultipleChoiceQuestion> _questions;

    public IReadOnlyList<MultipleChoiceQuestion> Questions => _questions.AsReadOnly();
    public MultipleChoiceOption CorrectOption { get; private set; }

    private MultipleChoiceExercise()
    {
    }

    public MultipleChoiceExercise(string title, int maxScore, Guid creatorId, DateTime dueDate, Guid groupId,
        List<MultipleChoiceQuestion> questions, MultipleChoiceOption correctOption) : base(
        title, maxScore, creatorId, dueDate, groupId)
    {
        if (questions.Any(q =>
                q.Options.DistinctBy(o => o.OptionKey).Count() == q.Options.Count))
        {
            // todo exception type
            throw new LangAppException("A question cannot contain duplicate option keys");
        }

        _questions = questions;
        CorrectOption = correctOption;
    }
}