using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;

namespace LangApp.Application.Assignments.Extensions;

public static class PronunciationActivityDetailsExtensions
{
    public static PronunciationActivityDetails ToValueObject(this PronunciationActivityDetailsDto dto)
    {
        return new PronunciationActivityDetails(
            dto.ReferenceText,
            Language.FromString(dto.Language)
        );
    }
}