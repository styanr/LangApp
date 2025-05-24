using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Factories.StudyGroups;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;

namespace LangApp.Tests.Integration.Helpers;

public static class TestGroupFactory
{
    public static StudyGroup CreateTestGroup(string name, Guid teacherId)
    {
        var factory = new StudyGroupFactory(new KeyGenerator());
        return factory.Create(name, Language.EnglishUS, teacherId);
    }

    public static StudyGroup AddMember(this StudyGroup group, Guid userId)
    {
        group.AddMembers([new Member(userId, group.Id)]);
        return group;
    }
}
