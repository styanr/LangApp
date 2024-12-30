using LangApp.Core.Common;
using LangApp.Core.Events;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public class StudyGroup : AggregateRoot
{
    private readonly HashSet<Guid> _members;

    public StudyGroup(string name, StudyGroupLanguage language, Guid ownerId)
    {
        Name = name;
        Language = language;
        OwnerId = ownerId;
        _members = [];
    }

    public StudyGroup(Guid id, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset dateDeleted,
        bool isDeleted, string name, StudyGroupLanguage language, Guid ownerId, IEnumerable<Guid> members) : base(id,
        dateCreated, dateUpdated, dateDeleted, isDeleted)
    {
        Name = name;
        Language = language;
        OwnerId = ownerId;
        _members = [..members];
    }

    public string Name { get; private set; }
    public StudyGroupLanguage Language { get; private set; }

    public Guid OwnerId { get; private set; }

    public IReadOnlyCollection<Guid> Members => _members;

    public void AddMembers(List<Guid> members)
    {
        var intersect = Members.Intersect(members).ToList();

        if (intersect.Count != 0) throw new StudyGroupAlreadyContainsMembers(intersect);

        _members.UnionWith(members);

        CreateEvent(new StudyGroupMembersAdded(this, members));
    }

    public void RemoveMembers(List<Guid> members)
    {
        var except = members.Except(Members).ToList();

        if (except.Count != 0) throw new StudyGroupCantRemoveNotMembers(except);

        _members.ExceptWith(members);

        CreateEvent(new StudyGroupRemovedMembers(this, members));
    }
}