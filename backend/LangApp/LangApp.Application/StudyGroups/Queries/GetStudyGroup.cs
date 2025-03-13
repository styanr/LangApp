using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.StudyGroups.Dto;

namespace LangApp.Application.StudyGroups.Queries;

public record GetStudyGroup(Guid Id) : IQuery<StudyGroupDto>;