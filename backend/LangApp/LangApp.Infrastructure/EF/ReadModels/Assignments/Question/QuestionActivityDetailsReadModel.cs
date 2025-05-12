namespace LangApp.Infrastructure.EF.Models.Assignments.Question;

public class QuestionActivityDetailsReadModel : ActivityDetailsReadModel
{
    public string Question { get; set; }
    public List<string> Answers { get; set; }
    public int MaxLength { get; set; }
}