using LangApp.Core.Entities.Assignments;

namespace LangApp.Core.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetAsync(Guid id);
    Task AddAsync(Assignment activity);
    Task UpdateAsync(Assignment activity);
    Task DeleteAsync(Assignment activity);
}
