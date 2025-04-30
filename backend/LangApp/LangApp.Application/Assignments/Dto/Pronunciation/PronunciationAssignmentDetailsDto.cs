namespace LangApp.Application.Assignments.Dto.Pronunciation;

public record PronunciationAssignmentDetailsDto(
    string Language,
    string ReferenceText
) : AssignmentDetailsDto;
