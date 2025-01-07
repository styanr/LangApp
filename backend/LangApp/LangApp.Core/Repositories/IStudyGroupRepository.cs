using LangApp.Core.Entities.StudyGroup;

namespace LangApp.Core.Repositories;

public interface IStudyGroupRepository
{
    Task<StudyGroup> GetAsync(Guid id);
    Task AddAsync(StudyGroup group);
    Task UpdateAsync(StudyGroup group);
    Task DeleteAsync(StudyGroup group);
}