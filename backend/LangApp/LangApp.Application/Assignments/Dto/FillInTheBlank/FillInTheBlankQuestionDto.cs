namespace LangApp.Application.Assignments.Dto.FillInTheBlank;

public record FillInTheBlankQuestionDto(
    string TemplateText,
    List<FillInTheBlankAnswerDto> Answers);