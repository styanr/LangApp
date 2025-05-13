namespace LangApp.Infrastructure.PronunciationAssessment.Audio;

public class BlobStorageAudioProvider : IAudioStreamProvider
{
    private readonly IAudioFetcher _audioFetcher;

    public BlobStorageAudioProvider(IAudioFetcher audioFetcher)
    {
        _audioFetcher = audioFetcher ?? throw new ArgumentNullException(nameof(audioFetcher));
    }

    public async Task<Stream> GetAudioStreamAsync(string fileUri)
    {
        var audioInfo = await _audioFetcher.FetchAudioStream(fileUri);
        return audioInfo.Stream;
    }
}
