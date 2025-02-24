using LangApp.Core.Entities.Lexicons;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Lexicons;

public class LexiconEntryFactory : ILexiconEntryFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public LexiconEntryFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    public LexiconEntry Create(Guid lexiconId, Term term, IEnumerable<Definition> definitions)
    {
        return new LexiconEntry(_keyGenerator.NewKey(), lexiconId, term, definitions);
    }
}