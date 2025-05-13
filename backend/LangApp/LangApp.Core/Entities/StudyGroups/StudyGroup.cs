using LangApp.Core.Common;
using LangApp.Core.Events.StudyGroups;
using LangApp.Core.Exceptions.StudyGroups;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.StudyGroups;

internal static class StudyGroupConstants
{
    public const int MaxMembers = 8;
}

public class StudyGroup : AggregateRoot
{
    private readonly HashSet<Member> _members = new();

    public string Name { get; private set; }
    public Language Language { get; private set; }

    public Guid OwnerId { get; private set; }

    public IReadOnlyCollection<Member> Members => _members;

    private StudyGroup()
    {
    }

    internal StudyGroup(Guid id, string name, Language language, Guid ownerId) : base(id)
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

    public void AddMembers(IEnumerable<Member> members)
    {
        var incoming = members as ICollection<Member> ?? members.ToList();
        if (!incoming.Any()) return;

        var intersect = _members.Intersect(incoming).ToList();
        if (intersect.Count != 0)
            throw new AlreadyContainsMembersException(intersect);

        if (incoming.Count + _members.Count > StudyGroupConstants.MaxMembers)
            throw new TooManyMembersException(incoming.Count + _members.Count, StudyGroupConstants.MaxMembers);

        _members.UnionWith(incoming);
        AddEvent(new StudyGroupMembersAdded(this, incoming.ToList()));
    }

    public void RemoveMembers(IEnumerable<Member> members)
    {
        var list = members.ToList();
        // var except = list.Except(Members).ToList();
        //
        // if (except.Count != 0) throw new CantRemoveMembersException(except);

        _members.ExceptWith(list);

        AddEvent(new StudyGroupRemovedMembers(this, list));
    }

    public void UpdateName(string name)
    {
        if (Name == name) return;

        Name = name;

        AddEvent(new StudyGroupNameUpdated(name));
    }

    public bool CanBeModifiedBy(Guid userId)
    {
        return OwnerId == userId;
    }

    public bool ContainsMember(Guid userId)
    {
        return _members.Select(m => m.UserId).Contains(userId);
    }
}
