using System.Text.Json.Serialization;
using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

[JsonDerivedType(typeof(MultipleChoiceActivitySubmissionDetailsDto))]
[JsonDerivedType(typeof(PronunciationActivitySubmissionDetailsDto))]
[JsonDerivedType(typeof(FillInTheBlankActivitySubmissionDetailsDto))]
[JsonDerivedType(typeof(QuestionActivitySubmissionDetailsDto))]
[JsonDerivedType(typeof(WritingActivitySubmissionDetailsDto))]
public record ActivitySubmissionDetailsDto(
    ActivityType Type
);
