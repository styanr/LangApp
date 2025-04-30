using System.Text.Json.Serialization;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;

namespace LangApp.Application.Assignments.Dto;

[JsonDerivedType(typeof(MultipleChoiceAssignmentDetailsDto))]
[JsonDerivedType(typeof(FillInTheBlankAssignmentDetailsDto))]
[JsonDerivedType(typeof(PronunciationAssignmentDetailsDto))]
public record AssignmentDetailsDto();
