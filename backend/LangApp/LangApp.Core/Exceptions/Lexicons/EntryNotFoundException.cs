using LangApp.Core.Common.Exceptions;

namespace LangApp.Core.Exceptions.Lexicons;

public class EntryNotFoundException : NotFoundException
{
    public string Expression { get; }

    public EntryNotFoundException(string expression) : base($"No entry found for: '{expression}'")
    {
        Expression = expression;
    }
}