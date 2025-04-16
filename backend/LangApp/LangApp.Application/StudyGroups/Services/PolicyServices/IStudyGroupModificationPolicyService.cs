namespace LangApp.Application.StudyGroups.Services.PolicyServices;

public interface IStudyGroupModificationPolicyService
{
    Task<bool> IsSatisfiedBy(Guid studyGroupId, Guid userId);
}