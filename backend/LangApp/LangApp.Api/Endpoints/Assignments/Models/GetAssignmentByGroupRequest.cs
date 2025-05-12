using LangApp.Api.Common.Models;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record GetAssignmentByGroupRequest(
    Guid GroupId
);
