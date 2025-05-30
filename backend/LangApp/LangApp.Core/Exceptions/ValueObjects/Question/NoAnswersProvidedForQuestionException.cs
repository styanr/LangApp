namespace LangApp.Core.Exceptions.ValueObjects.Question;

public class NoAnswersProvidedForQuestionException()
    : LangAppException("Question must have at least one answer provided.")
{
}