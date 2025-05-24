using LangApp.Core.Exceptions;

namespace LangApp.Infrastructure.PronunciationAssessment.Exceptions;

public class InvalidUriException : LangAppException
{
    public string Uri { get; }

    public InvalidUriException(string uri) : base($"Invalid URI: {uri}")
    {
        Uri = uri;
    }
}
