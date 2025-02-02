namespace LangApp.Core.Exceptions.Lexicons;

public class EntryNotFoundException : LangAppException
{
    public string Expression { get; }

    public EntryNotFoundException(string expression) : base($"No entry found for: '{expression}'")
    {
        Expression = expression;
    }
}