namespace LangApp.Core.Exceptions.Assignments;

public class InvalidPronunciationActivityReferenceTextException(string referenceText)
    : LangAppException($"The provided reference text is invalid: {referenceText}");

public class PronunciationActivityReferenceTextTooLongException(int length, int maxLength) :
    LangAppException(
        $"The provided reference text is too long: {length} characters, maximum allowed is {maxLength} characters.");