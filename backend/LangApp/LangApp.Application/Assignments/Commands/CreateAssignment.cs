using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Extensions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Entities.Assignments;
using LangApp.Core.Exceptions;
using LangApp.Core.Factories.Assignments;
using LangApp.Core.Repositories;

namespace LangApp.Application.Assignments.Commands;

public record CreateAssignment(
    string Name,
    Guid AuthorId,
    Guid GroupId,
    DateTime DueDate,
    List<ActivityDto> Activities
) : ICommand<Guid>;

public class CreateAssignmentHandler : ICommandHandler<CreateAssignment, Guid>
{
    private readonly IAssignmentRepository _repository;
    private readonly IAssignmentFactory _factory;
    private readonly IActivityFactory _activityFactory;
    private readonly IStudyGroupRepository _groupRepository;

    public CreateAssignmentHandler(IAssignmentRepository repository, IAssignmentFactory factory,
        IActivityFactory activityFactory, IStudyGroupRepository groupRepository)
    {
        _repository = repository;
        _factory = factory;
        _activityFactory = activityFactory;
        _groupRepository = groupRepository;
    }

    public async Task<Guid> HandleAsync(CreateAssignment command, CancellationToken cancellationToken)
    {
        var (name, authorId, groupId, dueDate, activities) = command;

        if (activities.Count == 0)
        {
            throw new LangAppException("At least one activity is required");
        }

        var group = await _groupRepository.GetAsync(groupId)
                    ?? throw new StudyGroupNotFoundException(groupId);

        if (!group.CanBeModifiedBy(authorId))
        {
            throw new UnauthorizedException(authorId, group);
        }

        List<Activity> activitiesDomainModel = new();

        foreach (var activityDto in activities)
        {
            var detailsDomainModel = activityDto.Details.ToValueObject();
            var activity = _activityFactory.Create(detailsDomainModel, activityDto.MaxScore);

            activitiesDomainModel.Add(activity);
        }

        var assignment = _factory.Create(name, authorId, groupId, dueDate, activitiesDomainModel);
        await _repository.AddAsync(assignment);

        return assignment.Id;
    }
}
