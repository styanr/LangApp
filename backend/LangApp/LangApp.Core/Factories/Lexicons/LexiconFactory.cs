using LangApp.Core.Entities.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Lexicons;

public class LexiconFactory
{
    public Lexicon Create(Guid ownerId, Language language, LexiconTitle title)
    {
        var dictionary = new Lexicon(ownerId, language, title);
        return dictionary;
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