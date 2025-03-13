namespace LangApp.Api.Endpoints.Lexicons.Models;

public record RemoveEntryRequest(
    Guid LexiconId,
    Guid EntryId
);