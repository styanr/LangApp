namespace LangApp.Core.Exceptions.AppDictionaries;

public class SubDictionaryNotFound : LangAppException
{
    public Guid Id { get; }

    public SubDictionaryNotFound(Guid id) : base($"Sub-dictionary with ID {id} not found")
    {
        Id = id;
    }
}