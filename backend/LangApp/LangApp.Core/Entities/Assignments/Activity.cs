using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.Exceptions.Assignments;
using LangApp.Core.ValueObjects.Assignments;
using Microsoft.VisualBasic;

namespace LangApp.Core.Entities.Assignments;

public class Activity : BaseEntity
{
    private Activity()
    {
    }

    internal Activity(
        Guid id,
        ActivityDetails details,
        int maxScore,
        ActivityType type) : base(id)
    {
        if (maxScore <= 0)
        {
            throw new AssignmentMaxScoreInvalidException(maxScore);
        }

        Details = details;
        MaxScore = maxScore;
        Type = type;
    }

    public int MaxScore { get; private set; }
    public ActivityType Type { get; private set; }
    public ActivityDetails Details { get; private set; }
    public int Order { get; private set; } = 0;

    public void UpdateOrder(int order)
    {
        Order = order;
    }
}
