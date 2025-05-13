using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Functions;

public class GetUploadSasUri
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedAudioExtensions = { ".wav" };

    private readonly ILogger<GetUploadSasUri> _logger;

    public GetUploadSasUri(ILogger<GetUploadSasUri> logger)
    {
        _logger = logger;
    }

    [Function("GetUploadSasUri")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        _logger.LogInformation("Received request for SAS upload URL.");

        string? originalFileName = req.Query["fileName"];

        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            _logger.LogWarning("Missing 'fileName' query parameter.");
            return new BadRequestObjectResult("Missing fileName query parameter.");
        }

        string extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        _logger.LogInformation("Original file name: {OriginalFileName}, Extension: {Extension}", originalFileName,
            extension);

        string containerName;
        if (AllowedImageExtensions.Contains(extension))
        {
            containerName = "images";
        }
        else if (AllowedAudioExtensions.Contains(extension))
        {
            containerName = "recordings";
        }
        else
        {
            _logger.LogWarning("Unsupported file type: {Extension}", extension);
            return new BadRequestObjectResult("Unsupported file type.");
        }

        string guidFileName = $"{Guid.NewGuid()}{extension}";
        _logger.LogInformation("Generated GUID file name: {GuidFileName}, Container: {ContainerName}", guidFileName,
            containerName);

        try
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                                      ?? throw new InvalidOperationException(
                                          "AzureWebJobsStorage environment variable is not set.");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            _logger.LogInformation("Ensured container '{ContainerName}' exists.", containerName);

            var blobClient = containerClient.GetBlobClient(guidFileName);
            var sasUri = GenerateSasUri(blobClient, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Generated SAS URI for blob: {BlobUri}", blobClient.Uri);

            return new OkObjectResult(new
            {
                originalFileName,
                blobFileName = guidFileName,
                uploadUri = sasUri
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating SAS URI.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    private static Uri GenerateSasUri(BlobClient blobClient, TimeSpan validFor)
    {
        if (!blobClient.CanGenerateSasUri)
            throw new InvalidOperationException(
                "Cannot generate SAS URI. Ensure the Storage account uses a Shared Key credential.");

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(validFor)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        return blobClient.GenerateSasUri(sasBuilder);
    }
}
