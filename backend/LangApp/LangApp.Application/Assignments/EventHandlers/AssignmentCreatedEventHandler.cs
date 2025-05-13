using LangApp.Application.Assignments.Jobs;
using LangApp.Application.Common.DomainEvents.Handlers;
using LangApp.Application.Common.Jobs;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Events.Assignments;
using LangApp.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Assignments.EventHandlers;

public class AssignmentCreatedEventHandler : IDomainEventHandler<AssignmentCreatedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IJobScheduler _jobScheduler;

    public AssignmentCreatedEventHandler(IServiceScopeFactory scopeFactory, IJobScheduler jobScheduler)
    {
        _scopeFactory = scopeFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(AssignmentCreatedEvent notification, CancellationToken cancellationToken)
    {
        List<NotifyUserNewAssignmentJobData> jobDataList;

        using (var scope = _scopeFactory.CreateScope())
        {
            var groupRepository = scope.ServiceProvider.GetRequiredService<IStudyGroupRepository>();
            var assignment = notification.Assignment;

            var studyGroup = await groupRepository.GetAsync(assignment.StudyGroupId)
                             ?? throw new StudyGroupNotFound(assignment.StudyGroupId);

            jobDataList = studyGroup.Members.Select(member => new NotifyUserNewAssignmentJobData(
                member.UserId,
                studyGroup.Name,
                assignment.Name,
                assignment.DueDate
            )).ToList();
        }

        foreach (var jobData in jobDataList)
        {
            _jobScheduler.Enqueue<NotifyUserNewAssignmentJob, NotifyUserNewAssignmentJobData>(job =>
                job.ExecuteAsync(jobData)
            );
        }
    }
}