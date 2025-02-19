using LangApp.Core.Entities.Lexicons;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Lexicons;

public class LexiconFactory : ILexiconFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public LexiconFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    public Lexicon Create(Guid ownerId, Language language, LexiconTitle title)
    {
        return new(_keyGenerator.NewKey(), ownerId, language, title);
    }

    public Lexicon Create(
        Guid ownerId,
        Language language,
        LexiconTitle title,
        List<LexiconEntry> entries)
    {
        var dictionary = new Lexicon(_keyGenerator.NewKey(), ownerId, language, title, entries);
        return dictionary;
    }
}