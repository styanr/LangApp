namespace LangApp.Core.Exceptions.Grading;

public class NoBlanksInActivityException()
    : LangAppException("Grading failed: activity contains no blanks to fill.")
{
}