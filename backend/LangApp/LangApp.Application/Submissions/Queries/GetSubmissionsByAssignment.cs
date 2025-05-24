using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Submissions.Dto;

namespace LangApp.Application.Submissions.Queries;

public record GetSubmissionsByAssignment(
    Guid AssignmentId,
    Guid UserId
) : PagedQuery<PagedResult<AssignmentSubmissionDto>>;
