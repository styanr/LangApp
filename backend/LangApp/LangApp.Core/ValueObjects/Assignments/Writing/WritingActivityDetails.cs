using LangApp.Core.Exceptions.ValueObjects.Writing;

namespace LangApp.Core.ValueObjects.Assignments.Writing;

public record WritingActivityDetails : ActivityDetails
{
    public override bool CanBeGradedAutomatically => false;
    public string Prompt { get; init; }
    public int MaxWords { get; init; } = 500;

    public WritingActivityDetails(string prompt, int maxWords)
    {
        if (maxWords is < 10 or > 500)
        {
            throw new InvalidWritingMaxWordsException(maxWords);
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new EmptyWritingPromptException();
        }

        Prompt = prompt;
        MaxWords = maxWords;
    }
}