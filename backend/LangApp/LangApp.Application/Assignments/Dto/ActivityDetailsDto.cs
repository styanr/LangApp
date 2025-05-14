using System.Text.Json.Serialization;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Application.Assignments.Dto.Question;
using LangApp.Application.Assignments.Dto.Writing;
using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "activityType")]
[JsonDerivedType(typeof(MultipleChoiceActivityDetailsDto), typeDiscriminator: "MultipleChoice")]
[JsonDerivedType(typeof(MultipleChoiceActivityRestrictedDetailsDto), typeDiscriminator: "MultipleChoiceRestricted")]
[JsonDerivedType(typeof(FillInTheBlankActivityDetailsDto), typeDiscriminator: "FillInTheBlank")]
[JsonDerivedType(typeof(FillInTheBlankActivityRestrictedDetailsDto), typeDiscriminator: "FillInTheBlankRestricted")]
[JsonDerivedType(typeof(PronunciationActivityDetailsDto), typeDiscriminator: "Pronunciation")]
[JsonDerivedType(typeof(QuestionActivityDetailsDto), typeDiscriminator: "Question")]
[JsonDerivedType(typeof(QuestionActivityRestrictedDetailsDto), typeDiscriminator: "QuestionRestricted")]
[JsonDerivedType(typeof(WritingActivityDetailsDto), typeDiscriminator: "Writing")]
public record ActivityDetailsDto;