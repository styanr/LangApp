namespace LangApp.Core.Common;

public class StudyGroupCantRemoveNotMembers : Exception
{
    public StudyGroupCantRemoveNotMembers(List<Guid> members) : base(
        "Study group already contains the following members: " + string.Join(", ", members))
    {
        MissingMembers = members;
    }

    public List<Guid> MissingMembers { get; private set; }
}