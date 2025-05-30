using LangApp.Core.Exceptions;
using LangApp.Core.Exceptions.ValueObjects.Submissions.Writing;
using LangApp.Core.Services.WordCounting;

namespace LangApp.Core.ValueObjects.Submissions.Writing;

public record WritingSubmissionDetails : SubmissionDetails
{
    public string Text { get; init; }
    private const int WordLimit = 500;

    public WritingSubmissionDetails(string text)
    {
        var wordCount = WordCounter.CountWords(text);
        if (wordCount > WordLimit)
        {
            throw new WritingSubmissionTooLongException(wordCount, WordLimit);
        }

        Text = text;
    }
}