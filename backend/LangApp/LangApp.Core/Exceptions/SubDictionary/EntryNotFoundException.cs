namespace LangApp.Core.Exceptions.SubDictionary;

public class EntryNotFoundException : LangAppException
{
    public string Expression { get; }

    public EntryNotFoundException(string expression) : base($"No entry found for: '{expression}'")
    {
        Expression = expression;
    }
}