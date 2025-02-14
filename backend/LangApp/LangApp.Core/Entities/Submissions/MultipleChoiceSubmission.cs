namespace LangApp.Core.Entities.Submissions;

public class MultipleChoiceSubmission : Submission
{
    private readonly List<string> _selectedOptions;
    public IReadOnlyList<string> SelectedOptions => _selectedOptions.AsReadOnly();

    private MultipleChoiceSubmission()
    {
    }

    internal MultipleChoiceSubmission(Guid exerciseId, Guid studentId, List<string> selectedOptions) : base(exerciseId,
        studentId)
    {
        _selectedOptions = selectedOptions;
    }
}