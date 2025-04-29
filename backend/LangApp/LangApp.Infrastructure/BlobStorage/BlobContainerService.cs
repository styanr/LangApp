using Azure;
using Azure.Storage.Blobs;
using LangApp.Infrastructure.BlobStorage.Exceptions;
using Microsoft.Extensions.Logging;

namespace LangApp.Infrastructure.BlobStorage;

public class BlobContainerService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobContainerService> _logger;

    public BlobContainerService(BlobServiceClient blobServiceClient, ILogger<BlobContainerService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public async Task<BlobContainerClient> GetOrCreateContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        try
        {
            if (!await containerClient.ExistsAsync())
            {
                await containerClient.CreateAsync();
                _logger.LogInformation("Created container {ContainerName}", containerName);
            }
            else
            {
                _logger.LogInformation("Container {ContainerName} already exists", containerName);
            }
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to create or get container {ContainerName}: {Message}", containerName,
                ex.Message);

            throw new BlobStorageException($"Failed to create or get container '{containerName}'.", ex);
        }

        return containerClient;
    }
}
