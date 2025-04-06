using System.Text.Json.Serialization;

namespace LangApp.Application.Assignments.Dto;

[JsonDerivedType(typeof(MultipleChoiceAssignmentDetailsDto))]
public record AssignmentDetailsDto();