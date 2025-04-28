using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Services;
using LangApp.Application.StudyGroups.Services.PolicyServices;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Extensions;
using LangApp.Core.Exceptions;
using LangApp.Core.Factories.Submissions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Application.Submissions.Commands;

public record CreatePronunciationSubmission(
    Guid AssignmentId,
    PronunciationSubmissionDetailsDto Details,
    Guid StudentId
) : ICommand<Guid>;

public class CreatePronunciationSubmissionHandler : ICommandHandler<CreatePronunciationSubmission, Guid>
{
    private readonly ISubmissionFactory _factory;
    private readonly ISubmissionRepository _repository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IStudyGroupAccessPolicyService _groupAccessPolicy;
    private readonly IBlobStorageService _blobStorageService;

    public CreatePronunciationSubmissionHandler(ISubmissionFactory factory, ISubmissionRepository repository,
        IAssignmentRepository assignmentRepository, IStudyGroupAccessPolicyService groupAccessPolicy,
        IBlobStorageService blobStorageService)
    {
        _factory = factory;
        _repository = repository;
        _assignmentRepository = assignmentRepository;
        _groupAccessPolicy = groupAccessPolicy;
        _blobStorageService = blobStorageService;
    }

    public async Task<Guid> HandleAsync(CreatePronunciationSubmission command, CancellationToken cancellationToken)
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

        string[] permittedContentTypes = ["audio/wav", "audio/wave"];
        if (!permittedContentTypes.Contains(details.ContentType))
        {
            throw new LangAppException("Invalid file type");
        }

        var blobUri = await _blobStorageService.UploadFileAsync(details.Recording);

        var domainDetails = new PronunciationSubmissionDetails(blobUri);

        var submission = _factory.CreatePronunciation(assignmentId, studentId, domainDetails);
        await _repository.AddAsync(submission);

        return submission.Id;
    }
}
