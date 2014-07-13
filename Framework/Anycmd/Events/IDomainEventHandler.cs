﻿namespace Anycmd.Events
{
    /// <summary>
    /// Represents the event handler for domain events.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event to be handled by current handler.</typeparam>
    public interface IDomainEventHandler<TDomainEvent> : IEventHandler<TDomainEvent>
        where TDomainEvent : class, IDomainEvent
    {

    }
}
