using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.StudyGroups;

internal sealed class PostgresStudyGroupRepository : IStudyGroupRepository
{
    private readonly DbSet<StudyGroup> _groups;
    private readonly WriteDbContext _context;

    public PostgresStudyGroupRepository(WriteDbContext context)
    {
        _context = context;
        _groups = context.StudyGroups;
    }

    public Task<StudyGroup?> GetAsync(Guid id)
    {
        return _context.StudyGroups.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task AddAsync(StudyGroup group)
    {
        await _groups.AddAsync(group);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StudyGroup group)
    {
        _groups.Update(group);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(StudyGroup group)
    {
        _groups.Remove(group);

        await _context.SaveChangesAsync();
    }
}