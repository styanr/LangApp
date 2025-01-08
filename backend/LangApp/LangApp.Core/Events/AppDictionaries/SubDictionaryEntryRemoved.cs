using LangApp.Core.Common;
using LangApp.Core.Entities.Dictionaries;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.AppDictionaries;

public record SubDictionaryEntryRemoved(
    AppDictionary Dictionary,
    Language SubDictionaryLanguage,
    string SubDictionaryTitle,
    string Expression) : IDomainEvent;