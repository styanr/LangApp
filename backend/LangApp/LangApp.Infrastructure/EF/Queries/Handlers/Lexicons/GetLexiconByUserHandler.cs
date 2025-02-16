using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Dto;
using LangApp.Application.Lexicons.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Lexicons;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Lexicons;

internal sealed class GetLexiconByUserHandler : IQueryHandler<GetLexiconsByUser, IEnumerable<LexiconSlimDto>>
{
    private readonly DbSet<LexiconReadModel> _lexicons;

    public GetLexiconByUserHandler(ReadDbContext context)
    {
        _lexicons = context.Lexicons;
    }

    public async Task<IEnumerable<LexiconSlimDto>?> HandleAsync(GetLexiconsByUser query)
    {
        return await _lexicons
            .Where(l => l.OwnerId == query.UserId)
            .Select(l => new LexiconSlimDto
            (
                l.Id,
                l.OwnerId,
                l.Language,
                l.Title
            )).AsNoTracking().ToListAsync();
    }
}