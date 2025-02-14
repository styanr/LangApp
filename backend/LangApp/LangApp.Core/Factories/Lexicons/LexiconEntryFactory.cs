using LangApp.Core.Entities.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Lexicons;

public class LexiconEntryFactory : ILexiconEntryFactory
{
    public LexiconEntry Create(Guid id, Term term, IEnumerable<Definition> definitions)
    {
        return new LexiconEntry(id, term, definitions);
    }
}