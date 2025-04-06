using LangApp.Core.Entities.Assignments;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Assignments;

internal sealed class PostgresAssignmentRepository : IAssignmentRepository
{
    private readonly WriteDbContext _context;
    private readonly DbSet<Assignment> _assignments;

    public PostgresAssignmentRepository(WriteDbContext context)
    {
        _context = context;
        _assignments = context.Assignments;
    }

    public Task<Assignment?> GetAsync(Guid id)
    {
        return _assignments
            .SingleOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Assignment assignment)
    {
        await _assignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Assignment assignment)
    {
        _assignments.Update(assignment);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Assignment assignment)
    {
        _assignments.Remove(assignment);

        await _context.SaveChangesAsync();
    }
}