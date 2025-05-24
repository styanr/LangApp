using LangApp.Application.Assignments.Dto;
using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;

namespace LangApp.Application.Assignments.Queries;

public record GetAssignmentsByUser(Guid UserId, bool ShowSubmitted = false, bool ShowOverdue = false)
    : PagedQuery<PagedResult<AssignmentByUserSlimDto>>;