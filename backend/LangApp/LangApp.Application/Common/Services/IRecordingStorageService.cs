namespace LangApp.Application.Common.Services;

public interface IRecordingStorageService
{
    Task<Uri> UploadRecordingAsync(Stream fileStream);
}
