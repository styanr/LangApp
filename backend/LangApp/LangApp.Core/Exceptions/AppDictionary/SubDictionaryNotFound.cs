namespace LangApp.Core.Exceptions.AppDictionary;

public class SubDictionaryNotFound(Guid id) : LangAppException($"Sub-dictionary with ID {id} not found");