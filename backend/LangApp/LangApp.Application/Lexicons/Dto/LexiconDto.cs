namespace LangApp.Application.Lexicons.Dto;

public record LexiconDto(
    Guid Id,
    Guid UserId,
    string Language,
    string Title,
    Dictionary<string, IEnumerable<string>> Entries);