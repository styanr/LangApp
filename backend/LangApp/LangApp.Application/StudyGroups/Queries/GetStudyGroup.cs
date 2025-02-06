using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Dto;

namespace LangApp.Application.StudyGroups.Queries;

public class GetStudyGroup : IQuery<StudyGroupDto>
{
    public Guid Id { get; set; }
}