using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.StudyGroups;

public class StudyGroupFactory : IStudyGroupFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public StudyGroupFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    public StudyGroup Create(string name, Language language, Guid ownerId) =>
        new StudyGroup(_keyGenerator.NewKey(), name, language, ownerId);
}