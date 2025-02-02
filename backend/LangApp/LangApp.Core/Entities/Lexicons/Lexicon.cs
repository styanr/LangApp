using LangApp.Core.Common;
using LangApp.Core.Events.LanguageDictionaries;
using LangApp.Core.Exceptions.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Lexicons;

public class Lexicon : AggregateRoot
{
    private readonly Dictionary<Expression, Definitions> _entries = new();

    public Guid UserId { get; private set; }
    public Language Language { get; private set; }
    public LexiconTitle Title { get; private set; }

    public IReadOnlyDictionary<Expression, Definitions> Entries =>
        _entries.ToDictionary(x => x.Key, x => x.Value);

    internal Lexicon(Guid userId, Language language, LexiconTitle title)
    {
        UserId = userId;
        Language = language;
        Title = title;
    }

    internal Lexicon(
        Guid id,
        Guid userId,
        Language language,
        LexiconTitle title,
        Dictionary<Expression, Definitions> entries) : base(id)
    {
        UserId = userId;
        Language = language;
        Title = title;
        _entries = entries;
    }

    public void AddEntry(Expression expression, Definitions definitions)
    {
        if (!_entries.TryAdd(expression, definitions))
        {
            throw new EntryAlreadyExistsException(expression.Value);
        }

        AddEvent(new LexiconEntryAdded(this, expression, definitions));
    }

    public void AddDefinition(Expression expression, Definition definition)
    {
        if (!_entries.TryGetValue(expression, out var definitions))
        {
            throw new EntryNotFoundException(expression.Value);
        }

        definitions.Add(definition);
        AddEvent(new LexiconDefinitionAdded(this, expression, definition));
    }

    public void RemoveDefinition(Expression expression, Definition definition)
    {
        if (!_entries.TryGetValue(expression, out var definitions))
        {
            throw new EntryNotFoundException(expression.Value);
        }

        definitions.Remove(definition);
        AddEvent(new LexiconDefinitionRemoved(this, expression, definition));
    }

    public void RemoveEntry(Expression expression)
    {
        if (!_entries.ContainsKey(expression))
        {
            throw new EntryNotFoundException(expression.Value);
        }

        _entries.Remove(expression);
        AddEvent(new LexiconEntryRemoved(this, expression));
    }
}