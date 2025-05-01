using System.Text.Json.Serialization;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;

namespace LangApp.Application.Assignments.Dto;

// TODO this is trash
[JsonDerivedType(typeof(MultipleChoiceAssignmentDetailsDto))]
[JsonDerivedType(typeof(MultipleChoiceAssignmentRestrictedDetailsDto))]
[JsonDerivedType(typeof(FillInTheBlankAssignmentDetailsDto))]
[JsonDerivedType(typeof(FillInTheBlankAssignmentRestrictedDetailsDto))]
[JsonDerivedType(typeof(PronunciationAssignmentDetailsDto))]
public record AssignmentDetailsDto();
