namespace LangApp.Infrastructure.EF.Models.Assignments.FillInTheBlank;

public class FillInTheBlankActivityDetailsReadModel : ActivityDetailsReadModel
{
    public List<FillInTheBlankAssignmentQuestionReadModel> Questions { get; set; }
}