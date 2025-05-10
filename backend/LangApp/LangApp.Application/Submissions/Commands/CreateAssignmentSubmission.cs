using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.StudyGroups.Services.PolicyServices;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Extensions;
using LangApp.Core.Entities.Submissions;
using LangApp.Core.Factories.Submissions;
using LangApp.Core.Repositories;

namespace LangApp.Application.Submissions.Commands;

public record CreateAssignmentSubmission(
    Guid AssignmentId,
    Guid UserId,
    List<ActivitySubmissionDetailsDto> ActivitySubmissionDtos
) : ICommand<Guid>;

public class CreateAssignmentSubmissionHandler : ICommandHandler<CreateAssignmentSubmission, Guid>
{
    private readonly IAssignmentSubmissionFactory _factory;
    private readonly IActivitySubmissionFactory _activitySubmissionFactory;
    private readonly IAssignmentSubmissionRepository _repository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IStudyGroupAccessPolicyService _groupAccessPolicy;

    public CreateAssignmentSubmissionHandler(IAssignmentSubmissionFactory factory,
        IActivitySubmissionFactory activitySubmissionFactory, IAssignmentSubmissionRepository repository,
        IAssignmentRepository assignmentRepository, IStudyGroupAccessPolicyService groupAccessPolicy)
    {
        _factory = factory;
        _activitySubmissionFactory = activitySubmissionFactory;
        _repository = repository;
        _assignmentRepository = assignmentRepository;
        _groupAccessPolicy = groupAccessPolicy;
    }

    public async Task<Guid> HandleAsync(CreateAssignmentSubmission command, CancellationToken cancellationToken)
    {
        var (assignmentId, userId, activitySubmissionDtos) = command;

        var assignment = await _assignmentRepository.GetAsync(assignmentId) ??
                         throw new AssignmentNotFound(assignmentId);

        var canAccessGroup = await _groupAccessPolicy.IsSatisfiedBy(assignment.StudyGroupId, userId);
        if (!canAccessGroup)
        {
            throw new UnauthorizedException(userId, assignment.StudyGroupId, "StudyGroup");
        }

        List<ActivitySubmission> activitySubmissions = new();

        foreach (var activitySubmissionDto in activitySubmissionDtos)
        {
            var detailsDomainModel = activitySubmissionDto.ToValueObject();

            var activitySubmissionDomainModel = _activitySubmissionFactory.Create(detailsDomainModel);

            activitySubmissions.Add(activitySubmissionDomainModel);
        }

        var assignmentSubmission = _factory.Create(assignmentId, userId, activitySubmissions);
        await _repository.AddAsync(assignmentSubmission);

        return assignmentSubmission.Id;
    }
}
