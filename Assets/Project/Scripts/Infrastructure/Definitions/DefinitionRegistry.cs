using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Interfaces;

namespace Project.Infrastructure.Definitions
{
    /// <summary>
    /// Concrete implementation of IDefinitionRegistry.
    /// Stores Core definition objects indexed by type and stable ID.
    /// Populated exclusively by DefinitionLoader during bootstrap.
    /// Locked after bootstrap completes — read-only for the rest of the session.
    /// </summary>
    public sealed class DefinitionRegistry : IDefinitionRegistry
    {
        // Outer key: definition type. Inner key: stable ID. Value: definition object.
        private readonly Dictionary<Type, Dictionary<string, object>> _entries =
            new Dictionary<Type, Dictionary<string, object>>();

        private bool _isLocked;

        // ─── Registration (bootstrap only) ───────────────────────────────────────

        /// <summary>
        /// Registers a definition under its stable ID. Called by DefinitionLoader only.
        /// Rejects null/empty IDs, duplicate IDs, and calls after Lock().
        /// All failures are logged via DebugLogger and silently skipped.
        /// </summary>
        public void Register<T>(string id, T definition) where T : class
        {
            if (_isLocked)
            {
                DebugLogger.LogError(DebugCategory.DataDefinitions,
                    $"[DefinitionRegistry] Attempted to register '{typeof(T).Name}' with ID '{id}' after the registry was locked. Registration rejected.");
                return;
            }

            if (string.IsNullOrEmpty(id))
            {
                DebugLogger.LogError(DebugCategory.DataDefinitions,
                    $"[DefinitionRegistry] Cannot register a '{typeof(T).Name}' definition with a null or empty ID. Registration skipped.");
                return;
            }

            if (definition == null)
            {
                DebugLogger.LogError(DebugCategory.DataDefinitions,
                    $"[DefinitionRegistry] Cannot register a null definition for ID '{id}' (type: {typeof(T).Name}). Registration skipped.");
                return;
            }

            Type type = typeof(T);

            if (!_entries.ContainsKey(type))
            {
                _entries[type] = new Dictionary<string, object>(StringComparer.Ordinal);
            }

            if (_entries[type].ContainsKey(id))
            {
                DebugLogger.LogError(DebugCategory.DataDefinitions,
                    $"[DefinitionRegistry] Duplicate ID '{id}' for type '{typeof(T).Name}'. The existing entry will be kept. Registration skipped.");
                return;
            }

            _entries[type][id] = definition;
        }

        /// <summary>
        /// Locks the registry. After this call, no further registrations are accepted.
        /// Called by DefinitionLoader after all domains have been loaded.
        /// </summary>
        public void Lock()
        {
            _isLocked = true;
        }

        // ─── IDefinitionRegistry ──────────────────────────────────────────────────

        /// <inheritdoc/>
        public T Get<T>(string id) where T : class
        {
            Type type = typeof(T);

            if (!_entries.TryGetValue(type, out Dictionary<string, object> domain))
            {
                return null;
            }

            if (!domain.TryGetValue(id, out object value))
            {
                return null;
            }

            return value as T;
        }

        /// <inheritdoc/>
        public IReadOnlyList<T> GetAll<T>() where T : class
        {
            Type type = typeof(T);

            if (!_entries.TryGetValue(type, out Dictionary<string, object> domain))
            {
                return Array.Empty<T>();
            }

            var result = new List<T>(domain.Count);

            foreach (object value in domain.Values)
            {
                if (value is T typed)
                {
                    result.Add(typed);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public bool Has<T>(string id) where T : class
        {
            Type type = typeof(T);

            if (!_entries.TryGetValue(type, out Dictionary<string, object> domain))
            {
                return false;
            }

            return domain.ContainsKey(id);
        }
    }
}
