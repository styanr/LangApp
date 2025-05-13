namespace LangApp.Core.Exceptions.StudyGroups;

public class TooManyMembersException : LangAppException
{
    public int Members { get; }
    public int MaxMembers { get; }

    public TooManyMembersException(int members, int maxMembers) : base($"Too many members: {members} > {maxMembers}")
    {
        Members = members;
        MaxMembers = maxMembers;
    }
}
