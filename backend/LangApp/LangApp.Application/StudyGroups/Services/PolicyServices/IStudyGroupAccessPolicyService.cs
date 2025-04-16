namespace LangApp.Application.StudyGroups.Services.PolicyServices;

public interface IStudyGroupAccessPolicyService
{
    Task<bool> IsSatisfiedBy(Guid studyGroupId, Guid userId);
}