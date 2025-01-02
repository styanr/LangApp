namespace LangApp.Core.Exceptions.Feed;

public class ItemAlreadyExistsException(Guid id) : LangAppException($"Item with ID {id} already exists");