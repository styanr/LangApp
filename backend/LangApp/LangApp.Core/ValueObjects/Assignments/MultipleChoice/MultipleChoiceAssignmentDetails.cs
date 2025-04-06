namespace LangApp.Core.ValueObjects.Assignments.MultipleChoice;

public record MultipleChoiceAssignmentDetails(List<MultipleChoiceQuestion> _questions) : AssignmentDetails
{
    private readonly List<MultipleChoiceQuestion> _questions = _questions;
    public IReadOnlyList<MultipleChoiceQuestion> Questions => _questions.AsReadOnly();
};