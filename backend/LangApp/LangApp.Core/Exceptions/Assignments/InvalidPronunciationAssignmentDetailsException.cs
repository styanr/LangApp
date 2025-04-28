namespace LangApp.Core.Exceptions.Assignments;

public class InvalidPronunciationAssignmentDetailsException(string referenceText)
    : LangAppException($"The provided reference text is invalid: {referenceText}");