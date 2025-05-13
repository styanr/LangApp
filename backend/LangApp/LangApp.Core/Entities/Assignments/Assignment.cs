using LangApp.Core.Common;
using LangApp.Core.Events.Assignments;
using LangApp.Core.Exceptions;

namespace LangApp.Core.Entities.Assignments;

public class Assignment : AggregateRoot
{
    private readonly List<Activity> _activities = [];

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyList<Activity> Activities => _activities;
    public Guid AuthorId { get; private set; }
    public Guid StudyGroupId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime DueDate { get; private set; }

    private Assignment()
    {
    }

    internal Assignment(Guid id, string name, string? description, Guid authorId, Guid studyGroupId,
        DateTime dueDate) : base(id)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateAuthorId(authorId);
        ValidateStudyGroupId(studyGroupId);
        ValidateDueDate(dueDate);

        Name = name;
        AuthorId = authorId;
        StudyGroupId = studyGroupId;
        CreatedAt = DateTime.UtcNow;
        DueDate = dueDate;
    }

    internal static Assignment Create(Guid id, string name, string? description, Guid authorId, Guid studyGroupId,
        DateTime dueDate)
    {
        var assignment = new Assignment(id, name, description, authorId, studyGroupId, dueDate);

        assignment.AddEvent(new AssignmentCreatedEvent(assignment));

        return assignment;
    }

    internal static Assignment Create(Guid id, string name, string? description, Guid authorId, Guid studyGroupId,
        DateTime dueDate, IEnumerable<Activity> activities)
    {
        var assignment = new Assignment(id, name, description, authorId, studyGroupId, dueDate);

        assignment.AddMultipleActivities(activities);

        assignment.AddEvent(new AssignmentCreatedEvent(assignment));
        return assignment;
    }

    public void AddMultipleActivities(IEnumerable<Activity> activities)
    {
        _activities.AddRange(activities);
        ReorderActivities();
    }

    public void AddActivity(Activity activity)
    {
        _activities.Add(activity);
        ReorderActivities();
    }

    public void RemoveActivity(Activity activity)
    {
        _activities.Remove(activity);
        ReorderActivities();
    }

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void UpdateDueDate(DateTime dueDate)
    {
        ValidateDueDate(dueDate);
        DueDate = dueDate;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new LangAppException("Name cannot be null or empty");
        }

        if (name.Length > 100)
        {
            throw new LangAppException("Name cannot be longer than 100 characters");
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description is not null && description.Length > 500)
        {
            throw new LangAppException("Description cannot be longer than 500 characters");
        }
    }

    private static void ValidateAuthorId(Guid authorId)
    {
        if (authorId == Guid.Empty)
        {
            throw new LangAppException("Author ID cannot be empty");
        }
    }

    private static void ValidateStudyGroupId(Guid studyGroupId)
    {
        if (studyGroupId == Guid.Empty)
        {
            throw new LangAppException("Study Group ID cannot be empty");
        }
    }

    private static void ValidateDueDate(DateTime dueDate)
    {
        if (dueDate <= DateTime.UtcNow)
        {
            throw new LangAppException("Due date cannot be in the past");
        }
    }

    public bool CanBeModifiedBy(Guid userId)
    {
        return AuthorId == userId;
    }

    private void ReorderActivities()
    {
        for (var i = 0; i < _activities.Count; i++)
        {
            _activities[i].UpdateOrder(i);
        }
    }
}
