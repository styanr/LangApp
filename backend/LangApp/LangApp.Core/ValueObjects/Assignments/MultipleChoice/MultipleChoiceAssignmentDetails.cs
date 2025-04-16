using System.Text.Json.Serialization;

namespace LangApp.Core.ValueObjects.Assignments.MultipleChoice;

public record MultipleChoiceAssignmentDetails(List<MultipleChoiceQuestion> Questions)
    : AssignmentDetails;