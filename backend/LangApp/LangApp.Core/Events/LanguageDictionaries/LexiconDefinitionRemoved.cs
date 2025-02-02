using LangApp.Core.Common;
using LangApp.Core.Entities.Dictionaries;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.LanguageDictionaries;

public record LexiconDefinitionRemoved(
    Lexicon Dictionary,
    Expression Expression,
    Definition Definition) : IDomainEvent;