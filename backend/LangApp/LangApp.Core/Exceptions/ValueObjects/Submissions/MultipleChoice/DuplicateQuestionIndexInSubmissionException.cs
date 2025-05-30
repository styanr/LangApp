namespace LangApp.Core.Exceptions.ValueObjects.Submissions.MultipleChoice;

public class DuplicateQuestionIndexInSubmissionException()
    : LangAppException("Answers must contain unique question indexes.")
{
}