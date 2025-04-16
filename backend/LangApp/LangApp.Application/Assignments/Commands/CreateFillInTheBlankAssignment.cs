using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Extensions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Factories.Assignments;
using LangApp.Core.Repositories;

namespace LangApp.Application.Assignments.Commands;

public record CreateFillInTheBlankAssignment(
    Guid AuthorId,
    Guid GroupId,
    DateTime DueTime,
    int MaxScore,
    FillInTheBlankAssignmentDetailsDto Details
) : ICommand<Guid>;

public class CreateFillInTheBlankAssignmentHandler : ICommandHandler<CreateFillInTheBlankAssignment, Guid>
{
    private readonly IAssignmentRepository _repository;
    private readonly IAssignmentFactory _factory;
    private readonly IStudyGroupRepository _groupRepository;

    public CreateFillInTheBlankAssignmentHandler(IAssignmentRepository repository,
        IStudyGroupRepository groupRepository,
        IAssignmentFactory factory)
    {
        _repository = repository;
        _groupRepository = groupRepository;
        _factory = factory;
    }

    public async Task<Guid> HandleAsync(CreateFillInTheBlankAssignment command, CancellationToken cancellationToken)
    {
        var (authorId, groupId, dueTime, maxScore, details) = command;

        var group = await _groupRepository.GetAsync(groupId)
                    ?? throw new StudyGroupNotFoundException(groupId);

        if (!group.CanBeModifiedBy(authorId))
        {
            throw new UnauthorizedException(authorId, group);
        }

        var detailsDomainModel = details.ToValueObject();
        var assignment = _factory.CreateFillInTheBlank(detailsDomainModel, authorId, groupId, dueTime, maxScore);
        await _repository.AddAsync(assignment);

        return assignment.Id;
    }
}