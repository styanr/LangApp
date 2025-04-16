namespace LangApp.Core.Exceptions.Assignments;

public class InvalidFillInTheBlankQuestionText(string message = "The Fill In The Blank question text cannot be empty.")
    : LangAppException(message);