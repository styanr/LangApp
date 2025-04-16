namespace LangApp.Application.Assignments.Dto.FillInTheBlank;

public record FillInTheBlankAssignmentDetailsDto(
    List<FillInTheBlankQuestionDto> Questions) : AssignmentDetailsDto;