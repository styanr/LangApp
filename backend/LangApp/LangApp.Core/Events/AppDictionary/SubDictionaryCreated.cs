using LangApp.Core.Common;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public record SubDictionaryCreated(Language SubDictionaryLanguage, string SubDictionaryTitle) : IDomainEvent;