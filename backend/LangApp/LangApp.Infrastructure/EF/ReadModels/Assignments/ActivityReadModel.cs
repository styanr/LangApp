using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Assignments;

namespace LangApp.Infrastructure.EF.Models.Assignments;

public class ActivityReadModel
{
    public Guid Id { get; set; }
    public int MaxScore { get; set; }
    public ActivityType Type { get; set; }
    public ActivityDetailsReadModel Details { get; set; }
    public Guid AssignmentId { get; set; }
    public int Order { get; set; }
}
