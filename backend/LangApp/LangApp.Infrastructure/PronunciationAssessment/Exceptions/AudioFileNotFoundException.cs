namespace LangApp.Infrastructure.PronunciationAssessment.Exceptions;

using LangApp.Core.Exceptions;

public class AudioFileNotFoundException : LangAppException
{
    public AudioFileNotFoundException(string container, string blobName)
        : base($"Audio file not found in container '{container}' with name '{blobName}'")
    {
    }

    public AudioFileNotFoundException(string container, string blobName, Exception innerException)
        : base($"Audio file not found in container '{container}' with name '{blobName}'", innerException)
    {
    }
}