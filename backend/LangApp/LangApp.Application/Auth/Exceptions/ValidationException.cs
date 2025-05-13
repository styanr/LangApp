using LangApp.Core.Exceptions;

namespace LangApp.Application.Auth.Exceptions;

public class ValidationException : LangAppException
{
    public IEnumerable<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors) : base(
        "Invalid data provided.")
    {
        Errors = errors;
    }
}
