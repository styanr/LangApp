using LangApp.Core.Exceptions;

namespace LangApp.Infrastructure.BlobStorage.Exceptions;

public class BlobStorageException : LangAppException
{
    public BlobStorageException(string message)
        : base(message)
    {
    }

    public BlobStorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
