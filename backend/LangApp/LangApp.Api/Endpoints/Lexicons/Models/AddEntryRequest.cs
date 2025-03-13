namespace LangApp.Api.Endpoints.Lexicons.Models;

public record AddEntryRequest(
    string Term,
    List<string> Definitions);