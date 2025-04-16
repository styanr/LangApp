namespace LangApp.Infrastructure.EF.Models.Assignments.FillInTheBlank;

public class FillInTheBlankAssignmentDetailsReadModel : AssignmentDetailsReadModel
{
    public List<FillInTheBlankAssignmentQuestionReadModel> Questions { get; set; }
}