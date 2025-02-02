using LangApp.Core.Common;
using LangApp.Core.Entities.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.LanguageDictionaries;

public record LexiconDefinitionAdded(
    Lexicon Dictionary,
    Expression Expression,
    Definition Definition) : IDomainEvent;