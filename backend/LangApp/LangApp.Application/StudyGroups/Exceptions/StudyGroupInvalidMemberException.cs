using LangApp.Core.Exceptions;

namespace LangApp.Application.StudyGroups.Exceptions;

public class StudyGroupInvalidMemberException : LangAppException
{
    public Guid StudyGroupId { get; }

    public StudyGroupInvalidMemberException(Guid studyGroupId) : base(
        $"The user with ID {studyGroupId} is a teacher or owns the study group.")
    {
        StudyGroupId = studyGroupId;
    }
}