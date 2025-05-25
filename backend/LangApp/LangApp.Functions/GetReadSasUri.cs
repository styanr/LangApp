using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using LangApp.Functions.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace LangApp.Functions;

public class ReadSasResponseModel
{
    public string BlobFileName { get; set; }
    public string ContainerName { get; set; }
    public string ReadUri { get; set; }
}

public class GetReadSasUri
{
    private readonly ILogger<GetReadSasUri> _logger;

    public GetReadSasUri(ILogger<GetReadSasUri> logger)
    {
        _logger = logger;
    }

    [Authorize]
    [Function("GetReadSasUri")]
    [OpenApiOperation(operationId: "GetReadSasUri", tags: ["File Access"])]
    [OpenApiParameter(name: "containerName", In = ParameterLocation.Query, Required = true, Type = typeof(string),
        Description = "The name of the container (must be 'images' or 'recordings')")]
    [OpenApiParameter(name: "blobFileName", In = ParameterLocation.Query, Required = true, Type = typeof(string),
        Description = "The name of the blob file to access")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(ReadSasResponseModel),
        Description = "Returns the blob file name, container name, and SAS URI for reading")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(string),
        Description = "Bad request - missing parameters or invalid container name")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(string),
        Description = "Container or blob file not found")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json",
        bodyType: typeof(string),
        Description = "Internal server error during SAS generation")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
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

        string[] allowedContainers = ["images", "recordings", "documents"];
        if (!allowedContainers.Contains(containerName.ToLowerInvariant()))
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

            // Check if a container exists
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

            return new OkObjectResult(new ReadSasResponseModel
            {
                BlobFileName = blobFileName,
                ContainerName = containerName,
                ReadUri = sasUri.AbsoluteUri
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