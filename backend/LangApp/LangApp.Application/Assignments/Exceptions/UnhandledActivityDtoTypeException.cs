using LangApp.Core.Exceptions;

namespace LangApp.Application.Assignments.Exceptions;

public class UnhandledActivityDtoTypeException(Type activityDtoType)
    : LangAppException($"Unhandled activity DTO type encountered: {activityDtoType.Name}")
{
    public Type ActivityDtoType { get; } = activityDtoType;
}