using LangApp.Core.Common;
using LangApp.Core.Exceptions.AppDictionary;

namespace LangApp.Core.Entities;

public class AppDictionary : AggregateRoot
{
    private readonly List<SubDictionary> _subDictionaries = new();

    public IReadOnlyCollection<SubDictionary> SubDictionaries => _subDictionaries.AsReadOnly();
    public Guid UserId { get; private set; }

    private AppDictionary(Guid userId)
    {
        UserId = userId;
    }

    public void AddSubDictionary(SubDictionary subDictionary)
    {
        _subDictionaries.Add(subDictionary);

        CreateEvent(new SubDictionaryCreated(subDictionary.Language, subDictionary.Title));
    }

    public void RemoveSubDictionary(Guid subDictionaryId)
    {
        var subDictionary = GetSubDictionary(subDictionaryId);

        _subDictionaries.Remove(subDictionary);

        CreateEvent(new SubDictionaryRemoved(subDictionary.Language, subDictionary.Title));
    }

    public void AddEntryToSubDictionary(Guid subDictionaryId, string expression, List<string> definitions)
    {
        var subDictionary = GetSubDictionary(subDictionaryId);
        subDictionary.AddEntry(expression, definitions);

        CreateEvent(new SubDictionaryEntryAdded(subDictionary.Language, subDictionary.Title, expression));
    }

    public void AddDefinitionToEntry(Guid subDictionaryId, string expression, string definition)
    {
        var subDictionary = GetSubDictionary(subDictionaryId);
        subDictionary.AddDefinition(expression, definition);

        CreateEvent(new SubDictionaryDefinitionAdded(subDictionary.Language, subDictionary.Title, expression,
            definition));
    }

    public void RemoveEntryFromSubDictionary(Guid subDictionaryId, string expression)
    {
        var subDictionary = GetSubDictionary(subDictionaryId);
        subDictionary.RemoveEntry(expression);

        CreateEvent(new SubDictionaryEntryRemoved(subDictionary.Language, subDictionary.Title, expression));
    }

    public void RemoveDefinitionFromSubDictionary(Guid subDictionaryId, string expression, string definition)
    {
        var subDictionary = GetSubDictionary(subDictionaryId);
        subDictionary.RemoveDefinition(expression, definition);

        CreateEvent(new SubDictionaryDefinitionRemoved(subDictionary.Language, subDictionary.Title, expression,
            definition));
    }

    private SubDictionary GetSubDictionary(Guid subDictionaryId)
    {
        var subDictionary = _subDictionaries.FirstOrDefault(sd => sd.Id == subDictionaryId);
        if (subDictionary is null)
        {
            throw new SubDictionaryNotFound(subDictionaryId);
        }

        return subDictionary;
    }
}