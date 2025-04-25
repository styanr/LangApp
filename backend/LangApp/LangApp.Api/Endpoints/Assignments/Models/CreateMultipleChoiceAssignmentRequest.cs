using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record CreateMultipleChoiceAssignmentRequest(
    Guid GroupId,
    DateTime DueTime,
    int MaxScore,
    MultipleChoiceAssignmentDetailsDto Details
);
