using LangApp.Core.Common;
using LangApp.Core.Events.Lexicons;
using LangApp.Core.Exceptions.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Lexicons;

public class Lexicon : AggregateRoot
{
    private readonly List<LexiconEntry> _entries = new();

    public Guid UserId { get; private set; }
    public Language Language { get; private set; }
    public LexiconTitle Title { get; private set; }

    public IReadOnlyList<LexiconEntry> Entries => _entries.AsReadOnly();

    private Lexicon()
    {
    }

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
        List<LexiconEntry> entries) : base(id)
    {
        UserId = userId;
        Language = language;
        Title = title;
        _entries = entries;
    }

    public void AddEntry(LexiconEntry entry)
    {
        if (_entries.Any(e => e.Id == entry.Id))
        {
            throw new EntryAlreadyExistsException(entry.Id.ToString());
        }

        _entries.Add(entry);
        AddEvent(new LexiconEntryAdded(this, entry));
    }

    public void AddDefinition(Guid entryId, Definition definition)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId)
                    ?? throw new EntryNotFoundException(entryId.ToString());

        entry.Add(definition);
        AddEvent(new LexiconDefinitionAdded(this, entry));
    }

    public void RemoveDefinition(Guid entryId, Definition definition)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId)
                    ?? throw new EntryNotFoundException(entryId.ToString());

        entry.Remove(definition);
        AddEvent(new LexiconDefinitionRemoved(this, entry));
    }

    public void RemoveEntry(Guid entryId)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId)
                    ?? throw new EntryNotFoundException(entryId.ToString());

        _entries.Remove(entry);
        AddEvent(new LexiconEntryRemoved(this, entry));
    }
}