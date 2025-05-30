namespace LangApp.Core.Exceptions.Assignments;

public class InvalidAssignmentStudyGroupIdException(Guid studyGroupId)
    : LangAppException($"Study Group ID \"{studyGroupId}\" is invalid. It cannot be an empty GUID.")
{
    public Guid StudyGroupId { get; } = studyGroupId;
}
