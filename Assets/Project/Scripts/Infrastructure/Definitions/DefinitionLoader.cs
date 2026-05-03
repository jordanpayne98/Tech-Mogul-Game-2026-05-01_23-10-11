using Project.Core.Debugging;
using Project.Core.Definitions.Research;
using Project.Infrastructure.Definitions.ScriptableObjects;
using UnityEngine;

namespace Project.Infrastructure.Definitions
{
    /// <summary>
    /// Loads all definition databases from Resources at bootstrap and registers
    /// their entries into the DefinitionRegistry. Locks the registry after all domains
    /// have been loaded, making it read-only for the remainder of the session.
    ///
    /// Not a MonoBehaviour. Constructed and called by GameBootstrapper.
    /// Future domain loading blocks are added to LoadAll() as new definition types are designed.
    /// The method grows linearly with domain count, which is acceptable for MVP.
    /// </summary>
    public sealed class DefinitionLoader
    {
        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Loads all domain definition databases from Resources, registers their entries,
        /// and locks the registry. Must be called exactly once during bootstrap.
        /// </summary>
        public void LoadAll(DefinitionRegistry registry)
        {
            LoadResearchDefinitions(registry);

            int totalCount = CountRegisteredDefinitions(registry);
            int typeCount  = CountRegisteredTypes(registry);

            registry.Lock();

            DebugLogger.Log(DebugCategory.DataDefinitions,
                $"[DefinitionLoader] Definition registry locked. {totalCount} definitions registered across {typeCount} type(s).");
        }

        // ─── Domain loading blocks ────────────────────────────────────────────────

        private static void LoadResearchDefinitions(DefinitionRegistry registry)
        {
            var database = Resources.Load<ResearchDefinitionDatabase>("ResearchDefinitionDatabase");

            if (database == null)
            {
                DebugLogger.LogWarning(DebugCategory.DataDefinitions,
                    "[DefinitionLoader] ResearchDefinitionDatabase not found at Resources/ResearchDefinitionDatabase.asset. " +
                    "No research project definitions will be registered.");
                return;
            }

            if (database.Entries == null || database.Entries.Count == 0)
            {
                DebugLogger.LogWarning(DebugCategory.DataDefinitions,
                    "[DefinitionLoader] ResearchDefinitionDatabase.Entries is empty. " +
                    "No research project definitions will be registered.");
                return;
            }

            int count = 0;

            foreach (ResearchProjectDefinitionAsset asset in database.Entries)
            {
                if (asset == null)
                {
                    DebugLogger.LogWarning(DebugCategory.DataDefinitions,
                        "[DefinitionLoader] ResearchDefinitionDatabase contains a null entry. Skipping.");
                    continue;
                }

                ResearchProjectDefinition definition = asset.ToDefinition();
                registry.Register<ResearchProjectDefinition>(definition.Id, definition);
                count++;
            }

            DebugLogger.Log(DebugCategory.DataDefinitions,
                $"[DefinitionLoader] Loaded {count} research project definition(s).");
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Counts total registered definitions across all types by probing known types.
        /// Extended when new domain types are added to LoadAll().
        /// </summary>
        private static int CountRegisteredDefinitions(DefinitionRegistry registry)
        {
            return registry.GetAll<ResearchProjectDefinition>().Count;
        }

        /// <summary>
        /// Counts the number of distinct types that have at least one registered definition.
        /// </summary>
        private static int CountRegisteredTypes(DefinitionRegistry registry)
        {
            int count = 0;

            if (registry.GetAll<ResearchProjectDefinition>().Count > 0)
            {
                count++;
            }

            return count;
        }
    }
}
