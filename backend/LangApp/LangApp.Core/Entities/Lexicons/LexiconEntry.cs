using LangApp.Core.Common;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Lexicons;

public class LexiconEntry : BaseEntity
{
    private LexiconEntry()
    {
    }

    private readonly List<Definition> _definitions = new();

    public Guid LexiconId { get; private set; }
    public Term Term { get; private set; }

    public IReadOnlyList<Definition> Definitions => _definitions.AsReadOnly();

    public LexiconEntry(Guid id, Guid lexiconId, Term term, IEnumerable<Definition> definitions) : base(id)
    {
        LexiconId = lexiconId;
        Term = term;
        var list = definitions.ToList();

        if (definitions == null || list.Count == 0)
        {
            throw new ArgumentNullException(nameof(definitions), "Definitions cannot be null or empty.");
        }

        _definitions.AddRange(list);
    }

    public void Add(Definition definition)
    {
        _definitions.Add(definition);
    }

    public void Remove(Definition definition)
    {
        _definitions.Remove(definition);
    }
}