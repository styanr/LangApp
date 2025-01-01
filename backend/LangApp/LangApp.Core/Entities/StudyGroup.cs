using LangApp.Core.Common;
using LangApp.Core.Events;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public class StudyGroup : AggregateRoot
{
    private readonly HashSet<Member> _members = new();

    public string Name { get; private set; }
    public StudyGroupLanguage Language { get; private set; }

    public Guid OwnerId { get; private set; }

    public IReadOnlyCollection<Member> Members => _members;

    internal StudyGroup(string name, StudyGroupLanguage language, Guid ownerId)
    {
        Name = name;
        Language = language;
        OwnerId = ownerId;
    }

    private StudyGroup(Guid id, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset dateDeleted,
        bool isDeleted, string name, StudyGroupLanguage language, Guid ownerId, IEnumerable<Member> members)
        : base(id, dateCreated, dateUpdated, dateDeleted, isDeleted)
    {
        Name = name;
        Language = language;
        OwnerId = ownerId;
        _members = [..members];
    }

    public void AddMembers(IEnumerable<Member> members)
    {
        var list = members.ToList();
        var intersect = Members.Intersect(list).ToList();

        if (intersect.Count != 0) throw new AlreadyContainsMembersException(intersect);

        _members.UnionWith(list);

        CreateEvent(new StudyGroupMembersAdded(this, list));
    }

    public void RemoveMembers(List<Member> members)
    {
        var except = members.Except(Members).ToList();

        if (except.Count != 0) throw new CantRemoveMembersException(except);

        _members.ExceptWith(members);

        CreateEvent(new StudyGroupRemovedMembers(this, members));
    }
}