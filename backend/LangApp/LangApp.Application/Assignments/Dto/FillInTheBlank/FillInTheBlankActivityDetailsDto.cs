using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto.FillInTheBlank;

public record FillInTheBlankActivityDetailsDto(
    List<FillInTheBlankQuestionDto> Questions) : ActivityDetailsDto(ActivityType.FillInTheBlank);

public record FillInTheBlankAnswerDto(
    List<string> AcceptableAnswers
);

public record FillInTheBlankQuestionDto(
    string TemplateText,
    List<FillInTheBlankAnswerDto> Answers);

public record FillInTheBlankActivityRestrictedDetailsDto(
    List<FillInTheBlankRestrictedQuestionDto> Questions) : ActivityDetailsDto(ActivityType.FillInTheBlank);

public record FillInTheBlankRestrictedQuestionDto(
    string TemplateText);
