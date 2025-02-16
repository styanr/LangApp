using LangApp.Core.Entities.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Lexicons;

public class LexiconFactory : ILexiconFactory
{
    public Lexicon Create(Guid ownerId, Language language, LexiconTitle title)
    {
        return new(ownerId, language, title);
    }

    public Lexicon Create(
        Guid ownerId,
        Guid id,
        Language language,
        LexiconTitle title,
        List<LexiconEntry> entries)
    {
        var dictionary = new Lexicon(ownerId, id, language, title, entries);
        return dictionary;
    }
}