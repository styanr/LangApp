using LangApp.Core.Exceptions;

namespace LangApp.Application.Auth.Exceptions;

public class InvalidCredentialsException(string message) : LangAppException(message)
{
    public InvalidCredentialsException() : this("Invalid credentials provided")
    {
    }
}