using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto.FillInTheBlank;

public record FillInTheBlankAnswerDto(
    List<string> AcceptableAnswers
);

public record FillInTheBlankActivityDetailsDto(
    string TemplateText,
    List<FillInTheBlankAnswerDto> Answers) : ActivityDetailsDto(ActivityType.FillInTheBlank);

public record FillInTheBlankActivityRestrictedDetailsDto(
    string TemplateText) : ActivityDetailsDto(ActivityType.FillInTheBlank);
