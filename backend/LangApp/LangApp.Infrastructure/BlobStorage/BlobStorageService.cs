using LangApp.Application.Common.Services;

namespace LangApp.Infrastructure.BlobStorage;

public class BlobStorageService : IBlobStorageService
{
    public Task<string> UploadFileAsync(Stream fileStream)
    {
        Console.WriteLine("UploadFileAsync");

        return Task.FromResult("");
    }
}
