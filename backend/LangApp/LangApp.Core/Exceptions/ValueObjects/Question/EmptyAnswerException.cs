namespace LangApp.Core.Exceptions.ValueObjects.Question;

public class EmptyAnswerException()
    : LangAppException("An answer cannot be empty or whitespace.")
{
}