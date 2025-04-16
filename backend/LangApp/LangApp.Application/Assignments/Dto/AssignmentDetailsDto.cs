using System.Text.Json.Serialization;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;

namespace LangApp.Application.Assignments.Dto;

[JsonDerivedType(typeof(MultipleChoiceAssignmentDetailsDto))]

[JsonDerivedType(typeof(FillInTheBlankAssignmentDetailsDto))]
public record AssignmentDetailsDto();