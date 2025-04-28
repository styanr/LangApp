namespace LangApp.Application.Common.Services;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileStream);
}
