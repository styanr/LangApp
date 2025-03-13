namespace LangApp.Api.Endpoints.Lexicons.Models;

public record CreateLexiconRequest(
    string Title,
    string Language
);