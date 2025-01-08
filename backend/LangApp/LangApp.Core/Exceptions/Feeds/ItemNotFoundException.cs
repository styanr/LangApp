namespace LangApp.Core.Exceptions.Feeds;

public class ItemNotFoundException : LangAppException
{
    public Guid Id { get; }

    public ItemNotFoundException(Guid id) : base($"Item with ID {id} not found")
    {
        Id = id;
    }
}