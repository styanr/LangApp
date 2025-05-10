using LangApp.Core.Entities.Assignments;

namespace LangApp.Core.Events.Assignments;

public record AssignmentCreatedEvent(Assignment assignment);