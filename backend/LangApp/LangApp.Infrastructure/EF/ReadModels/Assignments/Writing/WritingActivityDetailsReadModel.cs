namespace LangApp.Infrastructure.EF.Models.Assignments.Writing;

public class WritingActivityDetailsReadModel : ActivityDetailsReadModel
{
    public string Prompt { get; set; }
    public int MaxWords { get; set; }
}