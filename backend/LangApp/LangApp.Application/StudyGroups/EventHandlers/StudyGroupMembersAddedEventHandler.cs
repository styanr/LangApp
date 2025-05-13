using LangApp.Application.Common.DomainEvents.Handlers;
using LangApp.Application.Common.Jobs;
using LangApp.Application.StudyGroups.Jobs;
using LangApp.Application.StudyGroups.Jobs.JobData;
using LangApp.Core.Events.StudyGroups;

namespace LangApp.Application.StudyGroups.EventHandlers;

public class StudyGroupMembersAddedEventHandler : IDomainEventHandler<StudyGroupMembersAdded>
{
    private readonly IJobScheduler _jobScheduler;

    public StudyGroupMembersAddedEventHandler(
        IJobScheduler jobScheduler)
    {
        _jobScheduler = jobScheduler;
    }

    public Task Handle(StudyGroupMembersAdded notification, CancellationToken cancellationToken)
    {
        var group = notification.StudyGroup;
        foreach (var member in notification.Members)
        {
            _jobScheduler.Enqueue<NotifyUserAddedToGroupJob, NotifyGroupMembersJobData>(job =>
                job.ExecuteAsync(new(member.UserId, group.Id, group.Name, group.Language.DisplayName))
            );
        }

        return Task.CompletedTask;
    }
}
