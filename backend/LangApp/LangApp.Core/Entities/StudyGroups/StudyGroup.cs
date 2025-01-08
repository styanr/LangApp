using LangApp.Core.Common;
using LangApp.Core.Events.StudyGroups;
using LangApp.Core.Exceptions.StudyGroups;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.StudyGroups;

public class StudyGroup : AggregateRoot
{
    private readonly HashSet<Member> _members = new();

    public string Name { get; private set; }
    public Language Language { get; private set; }

    public Guid OwnerId { get; private set; }

    public IReadOnlyCollection<Member> Members => _members;

    internal StudyGroup(string name, Language language, Guid ownerId)
    {
        Name = name;
        Language = language;
        OwnerId = ownerId;
    }

    internal StudyGroup(Guid id, string name, Language language, Guid ownerId, IEnumerable<Member> members)
        : base(id)
    {
        Name = name;
        Language = language;
        OwnerId = ownerId;
        _members = [..members];
    }

    // probably use a simple list here
    public void AddMembers(IEnumerable<Member> members)
    {
        var list = members.ToList();
        var intersect = Members.Intersect(list).ToList();

        if (intersect.Count != 0) throw new AlreadyContainsMembersException(intersect);

        _members.UnionWith(list);

        AddEvent(new StudyGroupMembersAdded(this, list));
    }

    public void RemoveMembers(IEnumerable<Member> members)
    {
        var list = members.ToList();
        var except = list.Except(Members).ToList();

        if (except.Count != 0) throw new CantRemoveMembersException(except);

        _members.ExceptWith(list);

        AddEvent(new StudyGroupRemovedMembers(this, list));
    }
}