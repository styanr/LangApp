namespace LangApp.Core.Exceptions.ValueObjects.Submissions.Writing;

public class WritingSubmissionTooLongException(int wordCount, int wordLimit)
    : LangAppException($"Text is too long. Word count: {wordCount}, Limit: {wordLimit}.")
{
    public int WordCount { get; } = wordCount;
    public int WordLimit { get; } = wordLimit;
}