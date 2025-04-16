namespace LangApp.Core.Exceptions.Assignments;

public class InvalidFillInTheBlankQuestionAnswers : LangAppException
{
    public int AnswersCount { get; set; }
    public int BlankCount { get; set; }

    public InvalidFillInTheBlankQuestionAnswers(int answersCount, int blankCount) : base(
        $"Number of answers ({answersCount}) does not match the number of blanks ({blankCount}).")

    {
        AnswersCount = answersCount;
        BlankCount = blankCount;
    }
}