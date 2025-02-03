using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.StudyGroups;

public class StudyGroupFactory : IStudyGroupFactory
{
    public StudyGroup Create(string name, Language language, Guid ownerId) =>
        new StudyGroup(name, language, ownerId);
}