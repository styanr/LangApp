using LangApp.Core.Entities.Dictionaries;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Dictionaries;

public interface IDictionaryFactory
{
    LanguageDictionary Create(Language language, DictionaryTitle title);

    LanguageDictionary Create(Guid id, Language language, DictionaryTitle title,
        Dictionary<Expression, Definitions> entries);
}