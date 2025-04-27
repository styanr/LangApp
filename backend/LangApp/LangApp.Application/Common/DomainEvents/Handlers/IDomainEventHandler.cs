using LangApp.Core.Common;
using MediatR;

namespace LangApp.Application.Common.DomainEvents.Handlers;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;
