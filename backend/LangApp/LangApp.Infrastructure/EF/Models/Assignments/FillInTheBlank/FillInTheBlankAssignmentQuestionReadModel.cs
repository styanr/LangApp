namespace LangApp.Infrastructure.EF.Models.Assignments.FillInTheBlank;

public class FillInTheBlankAssignmentQuestionReadModel
{
    public string TemplateText { get; set; }
    public List<FillInTheBlankAnswerReadModel> Answers { get; set; }
}