using LangApp.Core.Entities.Dictionaries;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Dictionaries;

public class DictionaryFactory
{
    public LanguageDictionary Create(Language language, DictionaryTitle title)
    {
        var dictionary = new LanguageDictionary(language, title);
        return dictionary;
    }

    public LanguageDictionary Create(
        Guid id,
        Language language,
        DictionaryTitle title,
        Dictionary<Expression, Definitions> entries)
    {
        var dictionary = new LanguageDictionary(id, language, title, entries);
        return dictionary;
    }
}