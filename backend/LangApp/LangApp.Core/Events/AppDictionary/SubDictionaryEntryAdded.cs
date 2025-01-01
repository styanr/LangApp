using LangApp.Core.Common;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public record SubDictionaryEntryAdded(Language SubDictionaryLanguage, string SubDictionaryTitle, string Expression) : IDomainEvent;