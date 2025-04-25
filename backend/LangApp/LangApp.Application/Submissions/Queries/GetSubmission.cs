using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Submissions.Dto;

namespace LangApp.Application.Submissions.Queries;

public record GetSubmission(Guid Id, Guid UserId) : IQuery<SubmissionDto>;