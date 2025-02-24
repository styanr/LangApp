namespace LangApp.Api.Endpoints.Lexicons.Models;

public record AddEntryRequestModel(
    string Term,
    List<string> Definitions);