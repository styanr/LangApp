using LangApp.Core.Common;

namespace LangApp.Core.Entities.Exercises;

public abstract class Exercise : AggregateRoot
{
    public string Title { get; private set; }
    public int MaxScore { get; private set; }
    public Guid CreatorId { get; private set; }
    public Guid GroupId { get; private set; }
    public DateTime DueDate { get; private set; }

    internal Exercise(string title, int maxScore, Guid creatorId, DateTime dueDate, Guid groupId)
    {
        Title = title;
        MaxScore = maxScore;
        CreatorId = creatorId;
        DueDate = dueDate;
        GroupId = groupId;
    }
}