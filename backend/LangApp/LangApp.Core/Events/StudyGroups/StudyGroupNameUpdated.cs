using LangApp.Core.Common;

namespace LangApp.Core.Events.StudyGroups;

public record StudyGroupNameUpdated(string Name): IDomainEvent;