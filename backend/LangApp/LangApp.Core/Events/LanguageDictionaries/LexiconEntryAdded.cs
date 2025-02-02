using LangApp.Core.Common;
using LangApp.Core.Entities.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.LanguageDictionaries;

public record LexiconEntryAdded(
    Lexicon Dictionary,
    Expression Expression,
    Definitions Definitions) : IDomainEvent;