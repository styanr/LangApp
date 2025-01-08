using LangApp.Core.Common;
using LangApp.Core.Exceptions.SubDictionaries;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Dictionaries;

public class SubDictionary : BaseEntity
{
    private readonly Dictionary<string, List<string>> _entries = new();

    public Language Language { get; }
    public string Title { get; }

    public IReadOnlyDictionary<string, List<string>> Entries => _entries.AsReadOnly();

    internal SubDictionary(Language language, string title)
    {
        Language = language;
        Title = title;
    }

    internal SubDictionary(Guid id, Language language, string title, Dictionary<string, List<string>> entries) :
        base(id)
    {
        Language = language;
        Title = title;
        _entries = entries;
    }


    public void AddEntry(string expression, List<string> definitions)
    {
        if (string.IsNullOrEmpty(expression))
        {
            throw new ArgumentNullException(nameof(expression), "Expression cannot be null or empty.");
        }

        if (definitions is null || definitions.Count == 0)
        {
            throw new ArgumentNullException(nameof(definitions), "Definitions cannot be null or empty.");
        }

        if (!_entries.TryAdd(expression, definitions))
        {
            throw new EntryAlreadyExistsException(expression);
        }
    }

    public void AddDefinition(string expression, string definition)
    {
        if (string.IsNullOrEmpty(expression))
        {
            throw new ArgumentNullException(nameof(expression), "Expression cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(definition))
        {
            throw new ArgumentNullException(nameof(definition), "Definition cannot be null or empty.");
        }

        if (!_entries.TryGetValue(expression, out var entry))
        {
            throw new EntryNotFoundException(expression);
        }

        entry.Add(definition);
    }

    public void RemoveDefinition(string expression, string definition)
    {
        if (string.IsNullOrEmpty(expression))
        {
            throw new ArgumentNullException(nameof(expression), "Expression cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(definition))
        {
            throw new ArgumentNullException(nameof(definition), "Definition cannot be null or empty.");
        }

        if (!_entries.TryGetValue(expression, out var entry))
        {
            throw new EntryNotFoundException(expression);
        }

        entry.Remove(definition);
    }

    public void RemoveEntry(string expression)
    {
        if (string.IsNullOrEmpty(expression))
        {
            throw new ArgumentNullException(nameof(expression), "Expression cannot be null or empty.");
        }

        if (!_entries.ContainsKey(expression))
        {
            throw new EntryNotFoundException(expression);
        }

        _entries.Remove(expression);
    }
}