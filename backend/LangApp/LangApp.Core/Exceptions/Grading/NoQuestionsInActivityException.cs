namespace LangApp.Core.Exceptions.Grading;

public class NoQuestionsInActivityException()
    : LangAppException("Grading failed: activity contains no questions.")
{
}