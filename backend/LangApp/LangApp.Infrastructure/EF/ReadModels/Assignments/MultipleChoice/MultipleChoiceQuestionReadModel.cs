namespace LangApp.Infrastructure.EF.Models.Assignments.MultipleChoice;

public class MultipleChoiceQuestionReadModel
{
    public string Question { get; set; }
    public List<MultipleChoiceQuestionOptionReadModel> Options { get; set; }
    public int CorrectOptionIndex { get; set; }
}