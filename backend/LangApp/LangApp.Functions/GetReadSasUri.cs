using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LangApp.Functions;

public class GetReadSasUri
{
    private readonly ILogger<GetReadSasUri> _logger;

    public GetReadSasUri(ILogger<GetReadSasUri> logger)
    {
        _logger = logger;
    }

    [Function("GetReadSasUri")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        _logger.LogInformation("Received request for SAS read URL.");

        string? containerName = req.Query["containerName"];
        string? blobFileName = req.Query["blobFileName"];

        if (string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobFileName))
        {
            _logger.LogWarning("Missing required query parameters.");
            return new BadRequestObjectResult("Missing containerName and/or blobFileName query parameters.");
        }

        _logger.LogInformation("Request for blob: {BlobFileName} in container: {ContainerName}",
            blobFileName, containerName);

        if (containerName != "images" && containerName != "recordings")
        {
            _logger.LogWarning("Invalid container requested: {ContainerName}", containerName);
            return new BadRequestObjectResult("Invalid container name.");
        }

        try
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                                      ?? throw new InvalidOperationException(
                                          "AzureWebJobsStorage environment variable is not set.");

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Check if container exists
            if (!await containerClient.ExistsAsync())
            {
                _logger.LogWarning("Container '{ContainerName}' does not exist.", containerName);
                return new NotFoundObjectResult($"Container '{containerName}' not found.");
            }

            var blobClient = containerClient.GetBlobClient(blobFileName);

            if (!await blobClient.ExistsAsync())
            {
                _logger.LogWarning("Blob '{BlobFileName}' does not exist in container '{ContainerName}'.",
                    blobFileName, containerName);
                return new NotFoundObjectResult($"File '{blobFileName}' not found.");
            }

            var sasUri = GenerateSasUri(blobClient, TimeSpan.FromMinutes(15));

            _logger.LogInformation("Generated read SAS URI for blob: {BlobUri}", blobClient.Uri);

            return new OkObjectResult(new
            {
                blobFileName,
                containerName,
                readUri = sasUri
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating read SAS URI.");
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

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder);
    }
}
