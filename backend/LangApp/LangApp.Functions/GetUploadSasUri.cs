using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using LangApp.Functions.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace LangApp.Functions;

public class ResponseModel
{
    public string OriginalFileName { get; set; }
    public string BlobFileName { get; set; }
    public string UploadUri { get; set; }
}

public class GetUploadSasUri
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedAudioExtensions = [".wav"];
    private static readonly string[] AllowedDocumentExtensions = [".pdf"];

    private readonly ILogger<GetUploadSasUri> _logger;

    public GetUploadSasUri(ILogger<GetUploadSasUri> logger)
    {
        _logger = logger;
    }

    [Authorize]
    [Function("GetUploadSasUri")]
    [OpenApiOperation(operationId: "GetUploadSasUri", tags: ["File Upload"])]
    [OpenApiParameter(name: "fileName", In = ParameterLocation.Query, Required = true, Type = typeof(string),
        Description = "The name of the file to upload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(ResponseModel),
        Description = "Returns the original file name, blob file name, and SAS URI for uploading")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(string),
        Description = "Bad request - missing fileName parameter or unsupported file type")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json",
        bodyType: typeof(string),
        Description = "Internal server error during SAS generation")]
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
        else if (AllowedDocumentExtensions.Contains(extension))
        {
            containerName = "documents";
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
                ResponseModel
                {
                    OriginalFileName = originalFileName,
                    BlobFileName = guidFileName,
                    UploadUri = sasUri.AbsoluteUri
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
            ExpiresOn = DateTimeOffset.UtcNow.Add(validFor),
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        return blobClient.GenerateSasUri(sasBuilder);
    }
}