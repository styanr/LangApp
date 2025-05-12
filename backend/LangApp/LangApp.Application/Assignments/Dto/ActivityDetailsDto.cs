using System.Text.Json.Serialization;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Application.Assignments.Dto.Question;
using LangApp.Application.Assignments.Dto.Writing;
using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto;

[JsonDerivedType(typeof(MultipleChoiceActivityDetailsDto))]
[JsonDerivedType(typeof(MultipleChoiceActivityRestrictedDetailsDto))]
[JsonDerivedType(typeof(FillInTheBlankActivityDetailsDto))]
[JsonDerivedType(typeof(FillInTheBlankActivityRestrictedDetailsDto))]
[JsonDerivedType(typeof(PronunciationActivityDetailsDto))]
[JsonDerivedType(typeof(QuestionActivityDetailsDto))]
[JsonDerivedType(typeof(QuestionActivityRestrictedDetailsDto))]
[JsonDerivedType(typeof(WritingActivityDetailsDto))]
public record ActivityDetailsDto(
    ActivityType Type
);
