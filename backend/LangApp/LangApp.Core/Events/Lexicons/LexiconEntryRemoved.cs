using LangApp.Core.Common;
using LangApp.Core.Entities.Lexicons;

namespace LangApp.Core.Events.Lexicons;

public record LexiconEntryRemoved(
    Lexicon Dictionary,
    LexiconEntry Entry) : IDomainEvent;