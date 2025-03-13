using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Dto;

namespace LangApp.Application.StudyGroups.Queries;

public record GetStudyGroupsByUser(
    Guid UserId
) : PagedQuery<PagedResult<StudyGroupSlimDto>>;