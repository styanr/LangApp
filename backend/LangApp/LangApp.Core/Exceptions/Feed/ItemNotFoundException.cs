namespace LangApp.Core.Exceptions.Feed;

public class ItemNotFoundException(Guid id) : LangAppException($"Item with ID {id} not found");