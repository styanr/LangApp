using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto.Question;

public record QuestionActivityDetailsDto(string Question, List<string> Answers, int MaxLength)
    : ActivityDetailsDto;

public record QuestionActivityRestrictedDetailsDto(string Question, int MaxLength)
    : ActivityDetailsDto;