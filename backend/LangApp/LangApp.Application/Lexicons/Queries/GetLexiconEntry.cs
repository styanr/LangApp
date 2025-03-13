using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;

namespace LangApp.Application.Lexicons.Queries;

public record GetLexiconEntry(Guid LexiconId, Guid EntryId, Guid UserId) : IQuery<LexiconEntryDto>;