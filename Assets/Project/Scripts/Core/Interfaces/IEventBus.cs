using System;

namespace Project.Core.Interfaces
{
    /// <summary>
    /// Typed event bus interface for decoupled cross-domain notifications.
    /// Phase 2 code depends only on this interface. The concrete implementation
    /// (InMemoryEventBus) lives in Core/Events and is injected by Composition.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes an event to all registered subscribers of type T.
        /// </summary>
        /// <typeparam name="T">Event type. Must be a reference type.</typeparam>
        /// <param name="eventData">The event payload to deliver. Must not be null.</param>
        void Publish<T>(T eventData) where T : class;

        /// <summary>
        /// Registers a handler to receive events of type T.
        /// </summary>
        /// <typeparam name="T">Event type. Must be a reference type.</typeparam>
        /// <param name="handler">The callback to invoke when events of type T are published. Must not be null.</param>
        void Subscribe<T>(Action<T> handler) where T : class;

        /// <summary>
        /// Removes a previously registered handler for events of type T.
        /// Safe to call if the handler is not currently registered.
        /// </summary>
        /// <typeparam name="T">Event type. Must be a reference type.</typeparam>
        /// <param name="handler">The callback to remove.</param>
        void Unsubscribe<T>(Action<T> handler) where T : class;
    }
}
