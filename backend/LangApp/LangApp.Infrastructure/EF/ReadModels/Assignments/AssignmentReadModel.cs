using LangApp.Infrastructure.EF.Models.StudyGroups;

namespace LangApp.Infrastructure.EF.Models.Assignments;

public class AssignmentReadModel
{
    public Guid Id { get; set; }
    public List<ActivityReadModel> Activities { get; set; }
    public StudyGroupReadModel StudyGroup { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid AuthorId { get; set; }
    public Guid StudyGroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DueDate { get; set; }
}
