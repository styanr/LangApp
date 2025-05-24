using LangApp.Application.Assignments.Dto;
using LangApp.Application.Common.Queries.Abstractions;

namespace LangApp.Application.Assignments.Queries;

public record GetAssignmnentSubmissionsStatistics(Guid AssignmentId, Guid UserId) :
    IQuery<AssignmentSubmissionsStatisticsDto>;