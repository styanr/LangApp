using Hangfire;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Infrastructure.BlobStorage.Options;
using LangApp.Infrastructure.Hangfire;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace LangApp.Infrastructure.BlobStorage;

public static class Extensions
{
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var blobStorage = configuration.GetOptions<BlobStorageOptions>("Azure:BlobStorage");
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(blobStorage.ConnectionString);
        });

        services.AddSingleton<BlobContainerService>();
        services.AddSingleton<BlobStorageService>();
        services.AddSingleton<IRecordingStorageService, RecordingStorageService>();
        return services;
    }
}
