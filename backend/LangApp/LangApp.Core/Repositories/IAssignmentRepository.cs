using LangApp.Core.Entities.Assignments;

namespace LangApp.Core.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetAsync(Guid id);
    Task AddAsync(Assignment assignment);
    Task UpdateAsync(Assignment assignment);
    Task DeleteAsync(Assignment assignment);
}