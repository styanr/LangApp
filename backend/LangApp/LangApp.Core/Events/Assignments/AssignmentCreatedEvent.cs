using LangApp.Core.Common;
using LangApp.Core.Entities.Assignments;

namespace LangApp.Core.Events.Assignments;

public record AssignmentCreatedEvent(Assignment Assignment) : IDomainEvent;
