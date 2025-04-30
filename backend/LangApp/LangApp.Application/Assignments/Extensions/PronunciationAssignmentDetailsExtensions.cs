using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;

namespace LangApp.Application.Assignments.Extensions;

public static class PronunciationAssignmentDetailsExtensions
{
    public static PronunciationAssignmentDetails ToValueObject(this PronunciationAssignmentDetailsDto dto)
    {
        return new PronunciationAssignmentDetails(
            dto.ReferenceText,
            Language.FromString(dto.Language)
        );
    }
}
