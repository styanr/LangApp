using LangApp.Core.Exceptions;

namespace LangApp.Application.Auth.Exceptions;

public class RegisterValidationException : LangAppException
{
    public IEnumerable<string> Errors { get; }

    public RegisterValidationException(IEnumerable<string> errors) : base(
        "Unfortunately, you have provided invalid register data")
    {
        Errors = errors;
    }
}