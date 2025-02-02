using LangApp.Core.Entities.Dictionaries;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Dictionaries;

public class LexiconFactory
{
    public Lexicon Create(Language language, LexiconTitle title)
    {
        var dictionary = new Lexicon(language, title);
        return dictionary;
    }

    public Lexicon Create(
        Guid id,
        Language language,
        LexiconTitle title,
        Dictionary<Expression, Definitions> entries)
    {
        var dictionary = new Lexicon(id, language, title, entries);
        return dictionary;
    }
}