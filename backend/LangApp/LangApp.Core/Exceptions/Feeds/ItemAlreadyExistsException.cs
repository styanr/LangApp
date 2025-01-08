namespace LangApp.Core.Exceptions.Feeds;

public class ItemAlreadyExistsException : LangAppException
{
    public Guid Id { get; }

    public ItemAlreadyExistsException(Guid id) : base($"Item with ID {id} already exists")
    {
        Id = id;
    }
}