using LangApp.Core.Entities.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Lexicons;

public interface ILexiconFactory
{
    Lexicon Create(Guid ownerId, Language language, LexiconTitle title);

    Lexicon Create(Guid id, Guid ownerId, Language language, LexiconTitle title,
        Dictionary<Expression, Definitions> entries);
}