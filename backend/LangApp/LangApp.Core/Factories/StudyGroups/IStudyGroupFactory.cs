using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.StudyGroups;

public interface IStudyGroupFactory
{
    StudyGroup CreateEmpty(string name, Language language, Guid ownerId);
}