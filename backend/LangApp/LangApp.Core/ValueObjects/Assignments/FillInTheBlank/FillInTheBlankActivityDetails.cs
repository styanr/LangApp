namespace LangApp.Core.ValueObjects.Assignments.FillInTheBlank;

public record FillInTheBlankActivityDetails(List<FillInTheBlankQuestion> Questions) : ActivityDetails;