using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

public record FillInTheBlankActivitySubmissionDetailsDto(List<FillInTheBlankSubmissionAnswerDto> Answers)
    : ActivitySubmissionDetailsDto;

public record FillInTheBlankSubmissionAnswerDto(int Index, string Answer);