namespace LangApp.Core.Exceptions.SubDictionary;

public class EntryNotFoundException(string expression) : LangAppException($"No entry found for: '{expression}'");