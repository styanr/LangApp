namespace LangApp.Infrastructure.PronunciationAssessment.Exceptions;

using LangApp.Core.Exceptions;

public class InvalidAudioFileUriException : LangAppException
{
    public InvalidAudioFileUriException(string fileUri)
        : base($"Invalid audio file URI: '{fileUri}'")
    {
    }

    public InvalidAudioFileUriException(string fileUri, Exception innerException)
        : base($"Invalid audio file URI: '{fileUri}'", innerException)
    {
    }
}