namespace LangApp.Core.Exceptions.AppDictionary;

public class SubDictionaryNotFound : LangAppException
{
    public Guid Id { get; }

    public SubDictionaryNotFound(Guid id) : base($"Sub-dictionary with ID {id} not found")
    {
        Id = id;
    }
}