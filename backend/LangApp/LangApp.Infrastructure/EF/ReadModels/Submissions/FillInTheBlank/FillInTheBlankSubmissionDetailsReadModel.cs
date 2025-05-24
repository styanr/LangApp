namespace LangApp.Infrastructure.EF.Models.Submissions.FillInTheBlank;

public class FillInTheBlankSubmissionDetailsReadModel : SubmissionDetailsReadModel
{
    public List<FillInTheBlankSubmissionAnswerReadModel> Answers { get; set; }
}

public class FillInTheBlankSubmissionAnswerReadModel
{
    public int Index { get; set; }
    public string Answer { get; set; }
}
