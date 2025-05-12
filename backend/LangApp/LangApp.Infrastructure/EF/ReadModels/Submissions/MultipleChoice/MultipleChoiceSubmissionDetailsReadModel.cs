namespace LangApp.Infrastructure.EF.Models.Submissions.MultipleChoice;

public class MultipleChoiceSubmissionDetailsReadModel : SubmissionDetailsReadModel
{
    public List<MultipleChoiceAnswerReadModel> Answers { get; set; }
}

public class MultipleChoiceAnswerReadModel
{
    public int QuestionIndex { get; set; }
    public int ChosenOptionIndex { get; set; }
}
