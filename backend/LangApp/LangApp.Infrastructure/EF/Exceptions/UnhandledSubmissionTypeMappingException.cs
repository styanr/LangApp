using LangApp.Core.Exceptions;

namespace LangApp.Infrastructure.EF.Exceptions;

public class UnhandledSubmissionTypeMappingException(Type type)
    : LangAppException($"Unhandled activity type '{type}' encountered during DTO mapping.")
{
    public Type ActivityType { get; } = type;
}