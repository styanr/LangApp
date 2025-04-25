using System.Text.Json.Serialization;

namespace LangApp.Application.Submissions.Dto;

[JsonDerivedType(typeof(MultipleChoiceSubmissionDetailsDto))]
public record SubmissionDetailsDto();
