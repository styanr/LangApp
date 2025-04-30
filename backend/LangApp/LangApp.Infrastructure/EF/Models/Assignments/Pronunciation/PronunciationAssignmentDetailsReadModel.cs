using LangApp.Infrastructure.EF.Models.Language;

namespace LangApp.Infrastructure.EF.Models.Assignments.Pronunciation;

public class PronunciationAssignmentDetailsReadModel : AssignmentDetailsReadModel
{
    public LanguageReadModel Language { get; set; }
    public string ReferenceText { get; set; }
}
