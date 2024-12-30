namespace LangApp.Core.Common;

public class StudyGroupAlreadyContainsMembers : LangAppException
{
    public StudyGroupAlreadyContainsMembers(List<Guid> members) : base(
        "Study group already contains the following members: " + string.Join(", ", members))
    {
        ExistingMembers = members;
    }

    public List<Guid> ExistingMembers { get; private set; }
}