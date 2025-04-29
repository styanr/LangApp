using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LangApp.Infrastructure.BlobStorage.Exceptions;
using Microsoft.Extensions.Logging;

namespace LangApp.Infrastructure.BlobStorage;

public class BlobStorageService
{
    private readonly BlobContainerService _containerService;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(BlobContainerService containerService, ILogger<BlobStorageService> logger)
    {
        _containerService = containerService;
        _logger = logger;
    }

    public async Task<Uri> UploadFileAsync(string containerName, Stream contentStream,
        string contentType)
    {
        if (contentStream is null || contentStream.Length == 0)
        {
            throw new ArgumentException("Content stream must not be null or empty.", nameof(contentStream));
        }

        if (contentStream.CanSeek)
        {
            contentStream.Position = 0;
        }

        var blobName = Guid.NewGuid().ToString();

        try
        {
            var containerClient = await _containerService.GetOrCreateContainerAsync(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };

            await blobClient.UploadAsync(contentStream, uploadOptions);

            _logger.LogInformation("Uploaded blob {BlobName} to container {ContainerName}", blobName, containerName);

            return blobClient.Uri;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Blob upload failed for {BlobName} in container {ContainerName}: {Message}", blobName,
                containerName, ex.Message);
            throw new BlobStorageException($"Failed to upload blob '{blobName}' to container '{containerName}'.", ex);
        }
    }
}
