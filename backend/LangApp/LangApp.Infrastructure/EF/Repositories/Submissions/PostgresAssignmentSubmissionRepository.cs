using LangApp.Core.Entities.Submissions;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Submissions;

internal class PostgresAssignmentSubmissionRepository : IAssignmentSubmissionRepository
{
    private readonly DbSet<AssignmentSubmission> _submissions;
    private readonly WriteDbContext _context;

    public PostgresAssignmentSubmissionRepository(WriteDbContext context)
    {
        _context = context;
        _submissions = context.AssignmentSubmissions;
    }

    public Task<AssignmentSubmission?> GetAsync(Guid id)
    {
        return _submissions
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddAsync(AssignmentSubmission submission)
    {
        _submissions.Add(submission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AssignmentSubmission submission)
    {
        _submissions.Update(submission);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AssignmentSubmission submission)
    {
        _submissions.Remove(submission);
        await _context.SaveChangesAsync();
    }
}
