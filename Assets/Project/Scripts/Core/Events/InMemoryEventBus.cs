using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Interfaces;

namespace Project.Core.Events
{
    /// <summary>
    /// Domain-agnostic in-memory event bus. Stores handlers by event type and
    /// delivers published events synchronously to all registered subscribers.
    /// Injected by Composition into Application and Core services that require IEventBus.
    /// </summary>
    public sealed class InMemoryEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        // -------------------------------------------------------------------------
        // IEventBus
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public void Publish<T>(T eventData) where T : class
        {
            if (eventData == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    $"[InMemoryEventBus] Publish<{typeof(T).Name}> called with null eventData. Event not delivered.");
                return;
            }

            Type key = typeof(T);

            if (!_handlers.TryGetValue(key, out List<Delegate> handlers) || handlers.Count == 0)
            {
                DebugLogger.Log(DebugCategory.Simulation,
                    $"[InMemoryEventBus] Published {typeof(T).Name} — no subscribers.");
                return;
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[InMemoryEventBus] Published {typeof(T).Name} to {handlers.Count} subscriber(s).");

            // Iterate over a snapshot to avoid modification-during-iteration issues.
            Delegate[] snapshot = handlers.ToArray();

            foreach (Delegate handler in snapshot)
            {
                ((Action<T>)handler).Invoke(eventData);
            }
        }

        /// <inheritdoc/>
        public void Subscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    $"[InMemoryEventBus] Subscribe<{typeof(T).Name}> called with null handler.");
                return;
            }

            Type key = typeof(T);

            if (!_handlers.TryGetValue(key, out List<Delegate> handlers))
            {
                handlers = new List<Delegate>();
                _handlers[key] = handlers;
            }

            handlers.Add(handler);
        }

        /// <inheritdoc/>
        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
            {
                return;
            }

            Type key = typeof(T);

            if (!_handlers.TryGetValue(key, out List<Delegate> handlers))
            {
                return;
            }

            handlers.Remove(handler);
        }
    }
}
