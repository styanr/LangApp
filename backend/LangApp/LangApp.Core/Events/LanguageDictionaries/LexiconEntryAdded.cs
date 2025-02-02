using LangApp.Core.Common;
using LangApp.Core.Entities.Dictionaries;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.LanguageDictionaries;

public record LexiconEntryAdded(
    Lexicon Dictionary,
    Expression Expression,
    Definitions Definitions) : IDomainEvent;