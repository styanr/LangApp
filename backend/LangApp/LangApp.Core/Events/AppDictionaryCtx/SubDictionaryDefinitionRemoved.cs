using LangApp.Core.Common;
using LangApp.Core.Entities.Dictionary;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.AppDictionaryCtx;

public record SubDictionaryDefinitionRemoved(
    AppDictionary Dictionary,
    Language SubDictionaryLanguage,
    string SubDictionaryTitle,
    string Expression,
    string Definition) : IDomainEvent;