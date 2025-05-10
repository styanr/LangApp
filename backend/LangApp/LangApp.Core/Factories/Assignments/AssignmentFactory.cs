using LangApp.Core.Entities.Assignments;

namespace LangApp.Core.Factories.Assignments;

public class AssignmentFactory : IAssignmentFactory
{
    public Assignment Create(string name, Guid authorId, Guid studyGroupId, DateTime dueDate)
    {
        return new Assignment(Guid.NewGuid(), name, authorId, studyGroupId, dueDate);
    }

    public Assignment Create(string name, Guid authorId, Guid studyGroupId, DateTime dueDate,
        IEnumerable<Activity> activities)
    {
        return new Assignment(Guid.NewGuid(), name, authorId, studyGroupId, dueDate, activities);
    }
}
