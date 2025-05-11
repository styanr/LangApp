using LangApp.Core.Entities.Assignments;

namespace LangApp.Core.Factories.Assignments;

public class AssignmentFactory : IAssignmentFactory
{
    public Assignment Create(string name, string? description, Guid authorId, Guid studyGroupId, DateTime dueDate)
    {
        return Assignment.Create(Guid.NewGuid(), name, description, authorId, studyGroupId, dueDate);
    }

    public Assignment Create(string name, string? description, Guid authorId, Guid studyGroupId, DateTime dueDate,
        IEnumerable<Activity> activities)
    {
        return Assignment.Create(Guid.NewGuid(), name, description, authorId, studyGroupId, dueDate, activities);
    }
}
