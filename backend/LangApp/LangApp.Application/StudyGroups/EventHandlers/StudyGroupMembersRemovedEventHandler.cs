using LangApp.Application.Common.DomainEvents.Handlers;
using LangApp.Application.Common.Jobs;
using LangApp.Application.StudyGroups.Jobs;
using LangApp.Application.StudyGroups.Jobs.JobData;
using LangApp.Core.Events.StudyGroups;

namespace LangApp.Application.StudyGroups.EventHandlers;

public class StudyGroupMembersRemovedEventHandler : IDomainEventHandler<StudyGroupMembersRemoved>
{
    private readonly IJobScheduler _jobScheduler;

    public StudyGroupMembersRemovedEventHandler(
        IJobScheduler jobScheduler)
    {
        _jobScheduler = jobScheduler;
    }

    public Task Handle(StudyGroupMembersRemoved notification, CancellationToken cancellationToken)
    {
        var group = notification.StudyGroup;
        foreach (var member in notification.Members)
        {
            _jobScheduler.Enqueue<NotifyUserRemovedFromGroupJob, NotifyGroupMembersJobData>(job =>
                job.ExecuteAsync(new(member.UserId, group.Id, group.Name, group.Language.DisplayName))
            );
        }

        return Task.CompletedTask;
    }
}
