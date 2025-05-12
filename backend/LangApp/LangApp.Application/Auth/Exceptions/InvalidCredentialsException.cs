using LangApp.Core.Exceptions;

namespace LangApp.Application.Auth.Exceptions;

public class InvalidCredentialsException() : LangAppException("Invalid credentials provided");
