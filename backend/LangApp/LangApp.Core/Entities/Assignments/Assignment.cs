using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.Exceptions.Assignments;
using LangApp.Core.ValueObjects.Assignments;
using Microsoft.VisualBasic;

namespace LangApp.Core.Entities.Assignments;

// TODO: use value objects directly
public class Assignment : AggregateRoot
{
    private Assignment()
    {
    }

    internal Assignment(
        Guid id,
        AssignmentDetails details,
        Guid authorId,
        Guid groupId,
        DateTime dueTime,
        int maxScore,
        AssignmentType type) : base(id)
    {
        if (dueTime <= DateTime.UtcNow)
        {
            throw new AssignmentDueTimeInvalidException(dueTime);
        }

        DueTime = dueTime;

        if (maxScore <= 0)
        {
            throw new AssignmentMaxScoreInvalidException(maxScore);
        }

        Details = details;
        AuthorId = authorId;
        GroupId = groupId;
        DueTime = dueTime;
        MaxScore = maxScore;
        Type = type;
    }

    public Guid AuthorId { get; private set; }
    public Guid GroupId { get; private set; }
    public DateTime DueTime { get; private set; }
    public int MaxScore { get; private set; }
    public AssignmentType Type { get; private set; }
    public AssignmentDetails Details { get; private set; }

    public bool CanBeFullyAccessedBy(Guid userId)
    {
        return AuthorId == userId;
    }
}