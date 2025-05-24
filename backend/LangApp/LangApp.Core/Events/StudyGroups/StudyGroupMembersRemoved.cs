using LangApp.Core.Common;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.StudyGroups;

public record StudyGroupMembersRemoved(StudyGroup StudyGroup, List<Member> Members) : IDomainEvent;