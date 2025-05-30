using LangApp.Core.Exceptions;

namespace LangApp.Api.Common.Exceptions;

public class ConfigurationException(string message)
    : LangAppException(message);