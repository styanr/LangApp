namespace LangApp.Application.Submissions.Dto;

public record MultipleChoiceSubmissionDetailsDto(List<MultipleChoiceSubmissionAnswerDto> Answers)
    : SubmissionDetailsDto;

public record MultipleChoiceSubmissionAnswerDto(int ChosenOptionIndex);
