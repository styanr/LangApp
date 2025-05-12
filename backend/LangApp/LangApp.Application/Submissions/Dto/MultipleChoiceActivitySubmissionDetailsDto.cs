using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

public record MultipleChoiceActivitySubmissionDetailsDto(List<MultipleChoiceSubmissionAnswerDto> Answers)
    : ActivitySubmissionDetailsDto(ActivityType.MultipleChoice);

public record MultipleChoiceSubmissionAnswerDto(int QuestionIndex, int ChosenOptionIndex);
