using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;
using LangApp.Application.Lexicons.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Lexicons;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Lexicons;

internal sealed class GetLexiconHandler : IQueryHandler<GetLexicon, LexiconDto>
{
    private readonly DbSet<LexiconReadModel> _lexicons;

    public GetLexiconHandler(ReadDbContext context)
    {
        _lexicons = context.Lexicons;
    }

    public Task<LexiconDto?> HandleAsync(GetLexicon query)
    {
        return _lexicons
            .Include(l => l.Entries)
            .ThenInclude(e => e.Definitions)
            .Where(l => l.Id == query.Id)
            .Select(l => new LexiconDto
            (
                l.Id,
                l.UserId,
                l.Language,
                l.Title,
                l.Entries.Select(e => e.ToDto())
            )).AsNoTracking().SingleOrDefaultAsync();
    }
}