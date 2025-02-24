using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;

namespace LangApp.Application.Lexicons.Queries;

public record GetLexiconEntry(Guid LexiconId, Guid EntryId) : IQuery<LexiconEntryDto>;