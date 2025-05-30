namespace LangApp.Core.Exceptions.ValueObjects.Question;

public class EmptyQuestionTextException()
    : LangAppException("Question text cannot be empty or whitespace.")
{
}