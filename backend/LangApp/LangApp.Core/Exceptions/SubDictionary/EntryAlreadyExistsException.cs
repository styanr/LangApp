namespace LangApp.Core.Exceptions.SubDictionary;

public class EntryAlreadyExistsException(string expression)
    : LangAppException($"Entry already exists for: '{expression}'");