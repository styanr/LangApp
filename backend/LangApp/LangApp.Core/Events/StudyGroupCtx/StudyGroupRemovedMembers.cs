using LangApp.Core.Common;
using LangApp.Core.Entities.StudyGroup;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.StudyGroupCtx;

public record StudyGroupRemovedMembers(StudyGroup StudyGroup, List<Member> Members) : IDomainEvent;