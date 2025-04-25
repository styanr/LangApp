using LangApp.Core.Entities.Submissions;

namespace LangApp.Core.Repositories;

public interface ISubmissionRepository
{
    Task<Submission?> GetAsync(Guid id);
    Task AddAsync(Submission submission);
    Task UpdateAsync(Submission submission);
    Task DeleteAsync(Submission submission);
}
