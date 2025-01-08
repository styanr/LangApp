namespace LangApp.Core.Exceptions.SubDictionaries;

public class EntryNotFoundException : LangAppException
{
    public string Expression { get; }

    public EntryNotFoundException(string expression) : base($"No entry found for: '{expression}'")
    {
        Expression = expression;
    }
}