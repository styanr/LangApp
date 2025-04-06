using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Assignments;

namespace LangApp.Infrastructure.EF.Models.Assignments;

public class AssignmentReadModel
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid GroupId { get; set; }
    public DateTime DueTime { get; set; }
    public int MaxScore { get; set; }
    public AssignmentType Type { get; set; }
    public AssignmentDetailsReadModel Details { get; set; }
}