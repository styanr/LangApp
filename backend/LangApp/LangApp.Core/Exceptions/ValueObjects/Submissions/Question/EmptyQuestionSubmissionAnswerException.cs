namespace LangApp.Core.Exceptions.ValueObjects.Submissions.Question;

public class EmptyQuestionSubmissionAnswerException()
    : LangAppException("Answer cannot be empty.")
{
}