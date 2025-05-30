namespace LangApp.Core.Exceptions.ValueObjects.Question;

public class AnswerTooLongException(string answer, int maxLength)
    : LangAppException($"The answer \"{answer}\" is too long. It cannot be longer than {maxLength} characters.")
{
    public string Answer { get; } = answer;
    public int MaxLength { get; } = maxLength;
}