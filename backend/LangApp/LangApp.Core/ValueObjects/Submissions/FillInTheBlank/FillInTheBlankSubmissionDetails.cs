namespace LangApp.Core.ValueObjects.Submissions.FillInTheBlank;

public record FillInTheBlankSubmissionDetails(List<FillInTheBlankSubmissionAnswer> Answers) : SubmissionDetails;

public record FillInTheBlankSubmissionAnswer(int Index, string Answer);