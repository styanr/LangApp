using LangApp.Core.ValueObjects;

namespace LangApp.Core.Exceptions.StudyGroups;

public class CantRemoveMembersException : Exception
{
    public List<Member> MissingMembers { get; private set; }

    public CantRemoveMembersException(List<Member> members) : base(
        "Study group already contains the following members: " + string.Join(", ", members))
    {
        MissingMembers = members;
    }
}