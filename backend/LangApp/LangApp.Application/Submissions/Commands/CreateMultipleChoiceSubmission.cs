using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.StudyGroups.Services.PolicyServices;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Extensions;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Submissions;
using LangApp.Core.Repositories;

namespace LangApp.Application.Submissions.Commands;

public record CreateMultipleChoiceSubmission(
    Guid AssignmentId,
    MultipleChoiceSubmissionDetailsDto Details,
    Guid StudentId
) : ICommand<Guid>;

public class CreateMultipleChoiceSubmissionHandler : ICommandHandler<CreateMultipleChoiceSubmission, Guid>
{
    private readonly ISubmissionFactory _factory;
    private readonly ISubmissionRepository _repository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IStudyGroupAccessPolicyService _groupAccessPolicy;

    public CreateMultipleChoiceSubmissionHandler(ISubmissionFactory factory, ISubmissionRepository repository,
        IAssignmentRepository assignmentRepository, IStudyGroupAccessPolicyService groupAccessPolicy)
    {
        _factory = factory;
        _repository = repository;
        _assignmentRepository = assignmentRepository;
        _groupAccessPolicy = groupAccessPolicy;
    }

    public async Task<Guid> HandleAsync(CreateMultipleChoiceSubmission command, CancellationToken cancellationToken)
    {
        var (assignmentId, details, studentId) = command;

        var assignment = await _assignmentRepository.GetAsync(assignmentId) ??
                         throw new AssignmentNotFound(assignmentId);

        // TODO: technically an assignment access policy should be used here
        var canAccessGroup = await _groupAccessPolicy.IsSatisfiedBy(assignment.GroupId, studentId);
        if (!canAccessGroup)
        {
            throw new UnauthorizedException(studentId, assignment.GroupId, "StudyGroup");
        }

        var detailsDomainModel = details.ToValueObject();
        var submission = _factory.CreateMultipleChoice(assignmentId, studentId, detailsDomainModel);
        await _repository.AddAsync(submission);

        return submission.Id;
    }
}
