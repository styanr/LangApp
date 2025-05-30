namespace LangApp.Core.Exceptions.ValueObjects.Writing;

public class EmptyWritingPromptException()
    : LangAppException("Prompt cannot be empty or whitespace.")
{
}