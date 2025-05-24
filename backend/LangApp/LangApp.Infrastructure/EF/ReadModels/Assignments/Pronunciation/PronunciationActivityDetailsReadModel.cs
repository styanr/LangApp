using LangApp.Infrastructure.EF.Models.Language;

namespace LangApp.Infrastructure.EF.Models.Assignments.Pronunciation;

public class PronunciationActivityDetailsReadModel : ActivityDetailsReadModel
{
    public LanguageReadModel Language { get; set; }
    public string ReferenceText { get; set; }
    public bool AllowAssessment { get; set; }
    public bool AllowListening { get; set; }
}