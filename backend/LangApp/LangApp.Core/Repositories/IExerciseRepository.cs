using LangApp.Core.Entities.Exercises;

namespace LangApp.Core.Repositories;

public interface IExerciseRepository
{
    Task<Exercise?> GetAsync(Guid id);
    Task AddAsync(Exercise exercise);
    Task UpdateAsync(Exercise exercise);
    Task DeleteAsync(Exercise exercise);
}