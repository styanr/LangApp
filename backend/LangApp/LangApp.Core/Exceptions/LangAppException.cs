namespace LangApp.Core.Exceptions;

public class LangAppException : Exception
{
    // todo make protected
    protected LangAppException(string message) : base(message)
    {
    }

    protected LangAppException(string message, Exception innerException) : base(message, innerException)
    {
    }
}