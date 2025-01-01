using LangApp.Core.ValueObjects;

namespace LangApp.Core.Exceptions.StudyGroup;

public class AlreadyContainsMembersException : LangAppException
{
    public AlreadyContainsMembersException(List<Member> members) : base(
        "Study group already contains the following members: " +
        string.Join(", ", members.Select(m => m.UserId.ToString())))
    {
        ExistingMembers = members;
    }

    public List<Member> ExistingMembers { get; private set; }
}