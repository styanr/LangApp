using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record AddMultipleChoiceAssignmentRequest(
    Guid GroupId,
    DateTime DueTime,
    int MaxScore,
    MultipleChoiceAssignmentDetailsDto Details
);