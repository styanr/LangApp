using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;

namespace LangApp.Application.Lexicons.Queries;

public class GetLexicon : IQuery<LexiconDto>
{
    public Guid Id { get; set; }
}