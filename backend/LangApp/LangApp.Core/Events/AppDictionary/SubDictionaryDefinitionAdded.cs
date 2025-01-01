using LangApp.Core.Common;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public record SubDictionaryDefinitionAdded(Language SubDictionaryLanguage, string SubDictionaryTitle, string Expression, string Definition) : IDomainEvent;