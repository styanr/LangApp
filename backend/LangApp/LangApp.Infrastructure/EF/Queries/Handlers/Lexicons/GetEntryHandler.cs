using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;
using LangApp.Application.Lexicons.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Lexicons;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Lexicons;

internal sealed class GetEntryHandler : IQueryHandler<GetLexiconEntry, LexiconEntryDto>
{
    private readonly ReadDbContext _context;

    public GetEntryHandler(ReadDbContext context)
    {
        _context = context;
    }

    public async Task<LexiconEntryDto?> HandleAsync(GetLexiconEntry query)
    {
        // Query that fetches only the needed entry with its definitions
        // and verifies the lexicon belongs to the user in a single query
        var entry = await _context.Lexicons
            .Where(l => l.Id == query.LexiconId && l.UserId == query.UserId)
            .SelectMany(l => l.Entries.Where(e => e.Id == query.EntryId))
            .Include(e => e.Definitions)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        return entry?.ToDto();
    }
}
