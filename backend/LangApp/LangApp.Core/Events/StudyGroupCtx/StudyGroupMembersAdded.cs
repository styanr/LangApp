using LangApp.Core.Common;
using LangApp.Core.Entities.StudyGroup;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.StudyGroupCtx;

public record StudyGroupMembersAdded(StudyGroup StudyGroup, List<Member> Members) : IDomainEvent;