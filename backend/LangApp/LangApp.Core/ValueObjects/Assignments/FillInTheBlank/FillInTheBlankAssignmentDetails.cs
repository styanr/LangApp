namespace LangApp.Core.ValueObjects.Assignments.FillInTheBlank;

public record FillInTheBlankAssignmentDetails(List<FillInTheBlankQuestion> Questions) : AssignmentDetails;