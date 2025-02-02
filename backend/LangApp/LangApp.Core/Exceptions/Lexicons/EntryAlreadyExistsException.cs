namespace LangApp.Core.Exceptions.SubDictionaries;

public class EntryAlreadyExistsException : LangAppException
{
    public string Expression { get; }

    public EntryAlreadyExistsException(string expression) : base($"Entry already exists for: '{expression}'")
    {
        Expression = expression;
    }
}