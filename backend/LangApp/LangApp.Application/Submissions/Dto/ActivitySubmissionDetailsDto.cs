using System.Text.Json.Serialization;
using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "activityType")]
[JsonDerivedType(typeof(MultipleChoiceActivitySubmissionDetailsDto), typeDiscriminator: "MultipleChoice")]
[JsonDerivedType(typeof(PronunciationActivitySubmissionDetailsDto), typeDiscriminator: "Pronunciation")]
[JsonDerivedType(typeof(FillInTheBlankActivitySubmissionDetailsDto), typeDiscriminator: "FillInTheBlank")]
[JsonDerivedType(typeof(QuestionActivitySubmissionDetailsDto), typeDiscriminator: "Question")]
[JsonDerivedType(typeof(WritingActivitySubmissionDetailsDto), typeDiscriminator: "Writing")]
public record ActivitySubmissionDetailsDto;