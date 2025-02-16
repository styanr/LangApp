using LangApp.Core.Entities.Exercises;
using LangApp.Core.Repositories;

namespace LangApp.Infrastructure.EF.Repositories.Exercises;

public class PostgresExerciseRepository : IExerciseRepository
{
    public Task<Exercise?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Exercise exercise)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Exercise exercise)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Exercise exercise)
    {
        throw new NotImplementedException();
    }
}