namespace LangApp.Infrastructure.PronunciationAssessment.Exceptions;

using LangApp.Core.Exceptions;

public class MissingContainerSegmentException : LangAppException
{
    public MissingContainerSegmentException()
        : base("Missing container segment in audio file URI")
    {
    }

    public MissingContainerSegmentException(Exception innerException)
        : base("Missing container segment in audio file URI", innerException)
    {
    }
}