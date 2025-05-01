namespace LangApp.Application.Assignments.Dto.Pronunciation;

public record PronunciationAssignmentDetailsDto(
    string Language,
    string ReferenceText
) : AssignmentDetailsDto;

public record PronunciationAssignmentRestrictedDetailsDto(
    string Language,
    string ReferenceText
) : AssignmentDetailsDto;
