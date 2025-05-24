using LangApp.Api.Common.Models;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record GetAssignmentByGroupRequest(
    Guid GroupId,
    bool ShowSubmitted = false,
    bool ShowOverdue = false
);