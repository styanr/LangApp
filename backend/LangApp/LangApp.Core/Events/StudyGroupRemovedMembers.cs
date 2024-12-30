using LangApp.Core.Common;
using LangApp.Core.Entities;

namespace LangApp.Core.Events;

public record StudyGroupRemovedMembers(StudyGroup StudyGroup, List<Guid> Members) : IDomainEvent;