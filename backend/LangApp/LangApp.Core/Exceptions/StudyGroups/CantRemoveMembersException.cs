using LangApp.Core.ValueObjects;

namespace LangApp.Core.Exceptions.StudyGroups;

public class CantRemoveMembersException : LangAppException
{
    public List<Member> MissingMembers { get; private set; }

    public CantRemoveMembersException(List<Member> members) : base(
        "The following members are not part of the study group: " + string.Join(", ", members.Select(m => m.UserId)))
    {
        MissingMembers = members;
    }
}