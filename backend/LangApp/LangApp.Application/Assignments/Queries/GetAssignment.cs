using LangApp.Application.Assignments.Dto;
using LangApp.Application.Common.Queries.Abstractions;

namespace LangApp.Application.Assignments.Queries;

public record GetAssignment(Guid Id, Guid UserId) : IQuery<AssignmentDto>;