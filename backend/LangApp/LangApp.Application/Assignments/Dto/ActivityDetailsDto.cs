using System.Text.Json.Serialization;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto;

[JsonDerivedType(typeof(MultipleChoiceActivityDetailsDto))]
[JsonDerivedType(typeof(MultipleChoiceActivityRestrictedDetailsDto))]
[JsonDerivedType(typeof(FillInTheBlankActivityDetailsDto))]
[JsonDerivedType(typeof(FillInTheBlankActivityRestrictedDetailsDto))]
[JsonDerivedType(typeof(PronunciationActivityDetailsDto))]
public record ActivityDetailsDto(
    ActivityType Type
);
