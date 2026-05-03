using System.Collections.Generic;
using UnityEngine;

namespace Project.Infrastructure.Definitions.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject database holding references to all ResearchProjectDefinitionAsset instances.
    /// Loaded from Resources at bootstrap by DefinitionLoader.
    /// The asset must be placed at Assets/Resources/ResearchDefinitionDatabase.asset.
    ///
    /// Populate Entries in the Inspector by dragging individual ResearchProjectDefinitionAsset
    /// .asset files from Assets/Project/Data/Definitions/Research/.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ResearchDefinitionDatabase",
        menuName = "Project/Definitions/Research Definition Database")]
    public sealed class ResearchDefinitionDatabase : ScriptableObject
    {
        [SerializeField,
         Tooltip("All ResearchProjectDefinitionAsset entries to be loaded into the registry at bootstrap. " +
                 "Null entries are skipped with a warning.")]
        private List<ResearchProjectDefinitionAsset> _entries = new List<ResearchProjectDefinitionAsset>();

        /// <summary>
        /// All research project definition assets to be registered at bootstrap.
        /// Read by DefinitionLoader; must not be modified at runtime.
        /// </summary>
        public List<ResearchProjectDefinitionAsset> Entries => _entries;
    }
}
