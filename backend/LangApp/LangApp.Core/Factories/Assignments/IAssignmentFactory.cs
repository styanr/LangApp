using LangApp.Core.Entities.Assignments;

namespace LangApp.Core.Factories.Assignments;

public interface IAssignmentFactory
{
    Assignment Create(string name, string? description, Guid authorId, Guid studyGroupId, DateTime dueDate);

    Assignment Create(string name, string? description, Guid authorId, Guid studyGroupId, DateTime dueDate,
        IEnumerable<Activity> activities);
}
