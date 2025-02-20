using LangApp.Core.Exceptions;

namespace LangApp.Application.Common.Exceptions;

public abstract class NotFoundException(string message) : LangAppException(message);