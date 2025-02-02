using LangApp.Core.Entities.Dictionaries;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Dictionaries;

public interface ILexiconFactory
{
    Lexicon Create(Language language, LexiconTitle title);

    Lexicon Create(Guid id, Language language, LexiconTitle title,
        Dictionary<Expression, Definitions> entries);
}