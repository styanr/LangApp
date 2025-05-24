using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto.MultipleChoice;

public record MultipleChoiceActivityDetailsDto(List<MultipleChoiceQuestionDto> Questions)
    : ActivityDetailsDto;

public record MultipleChoiceQuestionDto(string Question, List<string> Options, int CorrectOptionIndex);

public record MultipleChoiceActivityRestrictedDetailsDto(List<MultipleChoiceRestrictedQuestionDto> Questions)
    : ActivityDetailsDto;

public record MultipleChoiceRestrictedQuestionDto(string Question, List<string> Options);