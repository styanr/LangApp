namespace LangApp.Core.Exceptions.Assignments;

public class AssignmentMaxScoreInvalidException(int maxScore)
    : LangAppException($"The following max score is invalid for the assignment: {maxScore}")
{
    public int MaxScore { get; } = maxScore;
}