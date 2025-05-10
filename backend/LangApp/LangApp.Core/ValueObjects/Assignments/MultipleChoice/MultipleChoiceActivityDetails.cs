using System.Text.Json.Serialization;

namespace LangApp.Core.ValueObjects.Assignments.MultipleChoice;

public record MultipleChoiceActivityDetails(List<MultipleChoiceQuestion> Questions)
    : ActivityDetails;