using LangApp.Core.Exceptions;

namespace LangApp.Core.Common.Exceptions;

public abstract class NotFoundException(string message) : LangAppException(message);