using LangApp.Core.Entities.Submissions;

namespace LangApp.Core.Repositories;

public interface IAssignmentSubmissionRepository
{
    Task<AssignmentSubmission?> GetAsync(Guid id);
    Task<bool> ExistsForAssignmentAsync(Guid assignmentId, Guid userId);
    Task AddAsync(AssignmentSubmission submission);
    Task UpdateAsync(AssignmentSubmission submission);
    Task DeleteAsync(AssignmentSubmission submission);
}
