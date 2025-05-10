using LangApp.Core.Common;
using LangApp.Core.Exceptions;

namespace LangApp.Core.Entities.Assignments;

public class Assignment : AggregateRoot
{
    private readonly List<Activity> _activities = [];

    public string Name { get; private set; }
    public IReadOnlyList<Activity> Activities => _activities;
    public Guid AuthorId { get; private set; }
    public Guid StudyGroupId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime DueDate { get; private set; }

    private Assignment()
    {
    }

    internal Assignment(Guid id, string name, Guid authorId, Guid studyGroupId, DateTime dueDate) : base(id)
    {
        ValidateName(name);
        ValidateAuthorId(authorId);
        ValidateStudyGroupId(studyGroupId);
        ValidateDueDate(dueDate);

        Name = name;
        AuthorId = authorId;
        StudyGroupId = studyGroupId;
        CreatedAt = DateTime.UtcNow;
        DueDate = dueDate;
    }

    internal Assignment(Guid id, string name, Guid authorId, Guid studyGroupId, DateTime dueDate,
        IEnumerable<Activity> activities) : this(id, name, authorId, studyGroupId, dueDate)
    {
        _activities.AddRange(activities);
    }

    public void AddActivity(Activity activity)
    {
        _activities.Add(activity);
    }

    public void RemoveActivity(Activity activity)
    {
        _activities.Remove(activity);
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
        if (dueDate < DateTime.UtcNow)
        {
            throw new LangAppException("Due date cannot be in the past");
        }
    }

    public bool CanBeModifiedBy(Guid userId)
    {
        return AuthorId == userId;
    }
}