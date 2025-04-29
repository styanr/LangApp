using LangApp.Application.Common.Services;

namespace LangApp.Infrastructure.BlobStorage;

public class RecordingStorageService : IRecordingStorageService
{
    private readonly BlobStorageService _blobStorageService;
    private const string ContainerName = "recordings";
    private const string ContentType = "audio/wav";

    public RecordingStorageService(BlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    public async Task<Uri> UploadRecordingAsync(Stream fileStream)
    {
        return await _blobStorageService.UploadFileAsync(ContainerName, fileStream, ContentType);
    }
}
