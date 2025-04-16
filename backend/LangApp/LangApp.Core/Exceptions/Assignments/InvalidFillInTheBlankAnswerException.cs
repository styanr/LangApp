namespace LangApp.Core.Exceptions.Assignments;

public class InvalidFillInTheBlankAnswerException : LangAppException
{
    public InvalidFillInTheBlankAnswerException(int index) : base($"The provided answer at index {index} cannot be empty")
    {
        Index = index;
    }

    public int Index { get; set; }
}