namespace LangApp.Core.Exceptions.ValueObjects.Writing;

public class InvalidWritingMaxWordsException(int maxWords)
    : LangAppException($"Max words '{maxWords}' is invalid. It must be between 10 and 500.")
{
    public int MaxWords { get; } = maxWords;
}