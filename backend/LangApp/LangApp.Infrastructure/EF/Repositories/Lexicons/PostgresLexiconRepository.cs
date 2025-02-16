using LangApp.Core.Entities.Lexicons;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Lexicons;

internal sealed class PostgresLexiconRepository : ILexiconRepository
{
    private readonly DbSet<Lexicon> _lexicons;
    private readonly WriteDbContext _context;

    public PostgresLexiconRepository(WriteDbContext context)
    {
        _context = context;
        _lexicons = context.Lexicons;
    }

    public Task<Lexicon?> GetAsync(Guid id)
    {
        return _lexicons.SingleOrDefaultAsync(l => l.Id == id);
    }

    public async Task AddAsync(Lexicon lexicon)
    {
        await _lexicons.AddAsync(lexicon);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lexicon lexicon)
    {
        _lexicons.Update(lexicon);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Lexicon lexicon)
    {
        _lexicons.Remove(lexicon);

        await _context.SaveChangesAsync();
    }
}