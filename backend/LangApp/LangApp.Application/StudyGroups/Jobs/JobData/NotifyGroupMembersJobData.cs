using LangApp.Application.Common.Jobs;

namespace LangApp.Application.StudyGroups.Jobs.JobData;

public record NotifyGroupMembersJobData(
    Guid UserId,
    Guid StudyGroupId,
    string StudyGroupName,
    string StudyGroupLanguage) : IJobData;