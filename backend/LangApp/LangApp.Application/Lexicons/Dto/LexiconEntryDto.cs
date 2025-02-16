namespace LangApp.Application.Lexicons.Dto;

public record LexiconEntryDto(Guid Id, Guid LexiconId, string Term, IEnumerable<string> Definitions);