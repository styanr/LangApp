using LangApp.Core.Entities.StudyGroups;

namespace LangApp.Core.Repositories;

public interface IStudyGroupRepository
{
    Task<StudyGroup> GetAsync(Guid id);
    Task AddAsync(StudyGroup group);
    Task UpdateAsync(StudyGroup group);
    Task DeleteAsync(StudyGroup group);
}