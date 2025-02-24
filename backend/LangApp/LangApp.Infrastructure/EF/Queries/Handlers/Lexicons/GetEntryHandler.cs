using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;
using LangApp.Application.Lexicons.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Lexicons;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Lexicons;

internal sealed class GetEntryHandler : IQueryHandler<GetLexiconEntry, LexiconEntryDto>
{
    private readonly DbSet<LexiconReadModel> _lexicons;

    public GetEntryHandler(ReadDbContext context)
    {
        _lexicons = context.Lexicons;
    }

    public async Task<LexiconEntryDto?> HandleAsync(GetLexiconEntry query)
    {
        var lexicon = await _lexicons
            .Include(l => l.Entries)
            .ThenInclude(e => e.Definitions)
            .Where(l => l.Id == query.LexiconId)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        var entry = lexicon?.Entries
            .SingleOrDefault(e => e.Id == query.EntryId)
            ?.ToDto();

        return entry;
    }
}