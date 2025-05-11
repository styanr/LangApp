using LangApp.Core.Exceptions;

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
            throw new LangAppException("Max words must be between 10 and 500");
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new LangAppException("Prompt cannot be empty");
        }

        Prompt = prompt;
        MaxWords = maxWords;
    }
}
