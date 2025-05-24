using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto.Pronunciation;

public record PronunciationActivityDetailsDto(
    string Language,
    string ReferenceText,
    bool AllowAssessment = true,
    bool AllowListening = true
) : ActivityDetailsDto;