using LangApp.Core.Common;
using LangApp.Core.Entities;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events;

public record StudyGroupMembersAdded(StudyGroup StudyGroup, List<Member> Members) : IDomainEvent;