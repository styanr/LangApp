namespace LangApp.Core.Exceptions.Feed;

public class ItemAlreadyExistsException : LangAppException
{
    public Guid Id { get; }

    public ItemAlreadyExistsException(Guid id) : base($"Item with ID {id} already exists")
    {
        Id = id;
    }
}