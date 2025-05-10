using System.Text.Json.Serialization;

namespace LangApp.Application.Submissions.Dto;

[JsonDerivedType(typeof(MultipleChoiceActivitySubmissionDetailsDto))]
[JsonDerivedType(typeof(PronunciationActivitySubmissionDetailsDto))]
public record ActivitySubmissionDetailsDto();
