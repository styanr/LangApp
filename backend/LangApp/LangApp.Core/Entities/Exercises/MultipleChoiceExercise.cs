using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects.Exercises;

namespace LangApp.Core.Entities.Exercises;

public class MultipleChoiceExercise : Exercise
{
    private readonly List<MultipleChoiceQuestion> _questions;

    public IReadOnlyList<MultipleChoiceQuestion> Questions => _questions.AsReadOnly();
    public MultipleChoiceOption CorrectOption { get; private set; }

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

//         if (answer is not MultipleChoiceAnswer multipleChoiceAnswer)
// {
//     throw new LangAppException("Invalid answer type");
// }

// if (multipleChoiceAnswer.SelectedOptions.Count != _questions.Count)
// {
//     throw new LangAppException("Invalid multiple choice answer count");
// }

// double correctOptions = _questions.Zip(multipleChoiceAnswer.SelectedOptions)
//     .Count(t => t.First.CorrectOption == t.Second);

// Percentage percentage = new(correctOptions / _questions.Count);

// return new Evaluation(percentage);