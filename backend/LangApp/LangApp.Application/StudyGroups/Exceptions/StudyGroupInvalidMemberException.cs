using LangApp.Core.Exceptions;

namespace LangApp.Application.StudyGroups.Exceptions;

public class StudyGroupInvalidMemberException : LangAppException
{
    public Guid StudyGroupId { get; }

    public StudyGroupInvalidMemberException(Guid studyGroupId) : base($"Cannot add member with ID {studyGroupId}.")
    {
        StudyGroupId = studyGroupId;
    }
}