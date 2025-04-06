namespace LangApp.Core.Exceptions.Assignments;

public class AssignmentMaxScoreInvalid(int maxScore)
    : LangAppException($"The following max score is invalid for the assignment: {maxScore}");