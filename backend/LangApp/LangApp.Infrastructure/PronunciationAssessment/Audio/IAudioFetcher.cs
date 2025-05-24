namespace LangApp.Infrastructure.PronunciationAssessment.Audio;

public interface IAudioFetcher
{
    public Task<AudioStreamInfo> FetchAudioStream(string fileUri);
}

public record AudioStreamInfo(Stream Stream);
