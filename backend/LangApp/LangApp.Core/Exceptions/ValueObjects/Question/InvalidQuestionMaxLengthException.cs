namespace LangApp.Core.Exceptions.ValueObjects.Question;

public class InvalidQuestionMaxLengthException(int maxLength)
    : LangAppException($"Max length '{maxLength}' is invalid. It must be between 0 and 100.")
{
    public int MaxLength { get; } = maxLength;
}