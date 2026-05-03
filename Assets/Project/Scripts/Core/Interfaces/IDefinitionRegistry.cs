using System.Collections.Generic;

namespace Project.Core.Interfaces
{
    /// <summary>
    /// Read-only interface for looking up static definitions by stable ID.
    /// Simulation code must use this interface — never depend on the registry implementation directly.
    /// All definitions are registered and locked during bootstrap. The registry is read-only after locking.
    /// </summary>
    public interface IDefinitionRegistry
    {
        /// <summary>
        /// Returns the definition of type T with the given stable ID, or null if not found.
        /// Callers must null-check the return value.
        /// </summary>
        T Get<T>(string id) where T : class;

        /// <summary>
        /// Returns all registered definitions of type T.
        /// Returns an empty list if no definitions of type T have been registered.
        /// Never returns null.
        /// </summary>
        IReadOnlyList<T> GetAll<T>() where T : class;

        /// <summary>
        /// Returns true if a definition of type T with the given stable ID exists in the registry.
        /// </summary>
        bool Has<T>(string id) where T : class;
    }
}
