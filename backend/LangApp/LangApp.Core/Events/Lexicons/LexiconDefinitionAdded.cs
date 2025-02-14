using LangApp.Core.Common;
using LangApp.Core.Entities.Lexicons;

namespace LangApp.Core.Events.Lexicons;

public record LexiconDefinitionAdded(
    Lexicon Dictionary,
    LexiconEntry Entry) : IDomainEvent;