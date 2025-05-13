namespace LangApp.Infrastructure.PronunciationAssessment.Audio;

public interface IAudioStreamProvider
{
    Task<Stream> GetAudioStreamAsync(string identifier);
}
