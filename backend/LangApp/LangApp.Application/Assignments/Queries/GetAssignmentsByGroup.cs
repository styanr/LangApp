using LangApp.Application.Assignments.Dto;
using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;

namespace LangApp.Application.Assignments.Queries;

public record GetAssignmentsByGroup(
    Guid GroupId,
    Guid UserId
) : PagedQuery<PagedResult<AssignmentDto>>;