using LangApp.Core.Entities.Exercises;

namespace LangApp.Core.Repositories;

public interface IExerciseRepository
{
    Task<Exercise?> GetAsync(Guid id);
    Task AddAsync(Exercise user);
    Task UpdateAsync(Exercise user);
    Task DeleteAsync(Exercise user);
}