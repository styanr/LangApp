namespace LangApp.Application.Assignments.Dto.FillInTheBlank;

public record FillInTheBlankAssignmentDetailsDto(
    List<FillInTheBlankQuestionDto> Questions) : AssignmentDetailsDto;

public record FillInTheBlankAnswerDto(
    List<string> AcceptableAnswers
);

public record FillInTheBlankQuestionDto(
    string TemplateText,
    List<FillInTheBlankAnswerDto> Answers);

public record FillInTheBlankAssignmentRestrictedDetailsDto(
    List<FillInTheBlankRestrictedQuestionDto> Questions) : AssignmentDetailsDto;

public record FillInTheBlankRestrictedQuestionDto(
    string TemplateText);
