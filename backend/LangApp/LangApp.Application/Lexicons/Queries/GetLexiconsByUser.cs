using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;

namespace LangApp.Application.Lexicons.Queries;

public class GetLexiconsByUser : IQuery<IEnumerable<LexiconSlimDto>>
{
    public Guid UserId { get; set; }
}