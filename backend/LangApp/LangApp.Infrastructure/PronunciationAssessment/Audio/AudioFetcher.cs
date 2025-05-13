using LangApp.Core.Exceptions;
using LangApp.Infrastructure.BlobStorage;

namespace LangApp.Infrastructure.PronunciationAssessment.Audio;

public class AudioFetcher : IAudioFetcher
{
    private readonly BlobStorageService _blobService;

    public AudioFetcher(BlobStorageService blobService)
    {
        _blobService = blobService;
    }

    public async Task<AudioStreamInfo> FetchAudioStream(string fileUri)
    {
        var uri = new Uri(fileUri);
        var blobName = uri.Segments.Last();
        var container = uri.Segments.Skip(1).FirstOrDefault()?.TrimEnd('/')
                        ?? throw new LangAppException("Missing container segment");
        if (!await _blobService.Exists(container, blobName))
            throw new LangAppException("File not found");
        var stream = await _blobService.DownloadFileAsync(container, blobName);
        return new AudioStreamInfo(stream);
    }
}
