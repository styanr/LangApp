using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;

namespace LangApp.Application.Lexicons.Queries;

public record GetLexiconsByUser(Guid UserId) : IQuery<IEnumerable<LexiconSlimDto>>;