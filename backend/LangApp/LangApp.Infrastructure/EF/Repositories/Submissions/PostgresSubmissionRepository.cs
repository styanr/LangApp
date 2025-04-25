using LangApp.Core.Entities.Submissions;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Submissions;

internal class PostgresSubmissionRepository : ISubmissionRepository
{
    private readonly DbSet<Submission> _submissions;
    private readonly WriteDbContext _context;

    public PostgresSubmissionRepository(WriteDbContext context)
    {
        _context = context;
        _submissions = context.Submissions;
    }

    public Task<Submission?> GetAsync(Guid id)
    {
        return _submissions
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddAsync(Submission submission)
    {
        _submissions.Add(submission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Submission submission)
    {
        _submissions.Update(submission);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Submission submission)
    {
        _submissions.Remove(submission);
        await _context.SaveChangesAsync();
    }
}
