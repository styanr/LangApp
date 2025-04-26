using LangApp.Core.Exceptions;

namespace LangApp.Application.Auth.Exceptions;

public class InvalidCredentialsException() : LangAppException("You have provided invalid credentials.");