using System.Text.Json.Serialization;

namespace LangApp.Application.Submissions.Dto;

[JsonDerivedType(typeof(MultipleChoiceSubmissionDetailsDto))]
[JsonDerivedType(typeof(PronunciationSubmissionDetailsDto))]
public record SubmissionDetailsDto();
