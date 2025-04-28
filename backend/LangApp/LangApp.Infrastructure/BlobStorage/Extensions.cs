using Hangfire;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Infrastructure.Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Infrastructure.BlobStorage;

public static class Extensions
{
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services)
    {
        services.AddScoped<IBlobStorageService, BlobStorageService>(); // TODO TEMP

        return services;
    }
}
