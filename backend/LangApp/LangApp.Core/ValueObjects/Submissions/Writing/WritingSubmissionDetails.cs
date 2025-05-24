using LangApp.Core.Exceptions;
using LangApp.Core.Services.WordCounting;

namespace LangApp.Core.ValueObjects.Submissions.Writing;

public record WritingSubmissionDetails : SubmissionDetails
{
    public string Text { get; init; }
    private const int WordLimit = 500;

    public WritingSubmissionDetails(string text)
    {
        if (WordCounter.CountWords(text) > WordLimit)
        {
            throw new LangAppException("Text is too long");
        }

        Text = text;
    }
}
