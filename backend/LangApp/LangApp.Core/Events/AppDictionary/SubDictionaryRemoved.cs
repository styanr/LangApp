using LangApp.Core.Common;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public record SubDictionaryRemoved(Language SubDictionaryLanguage, string SubDictionaryTitle) : IDomainEvent;