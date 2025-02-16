using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Dto;

namespace LangApp.Application.StudyGroups.Queries;

public class GetStudyGroupsByUser : PagedQuery<PagedResult<StudyGroupSlimDto>>
{
    public Guid UserId { get; set; }
}