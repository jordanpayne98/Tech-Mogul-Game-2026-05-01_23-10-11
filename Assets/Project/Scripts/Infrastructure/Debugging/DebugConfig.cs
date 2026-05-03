using Project.Core.Debugging;
using Project.Core.Interfaces;
using UnityEngine;

namespace Project.Infrastructure.Debugging
{
    /// <summary>
    /// ScriptableObject that implements IDebugConfig with inspector-editable toggles.
    /// Place the asset at Assets/Resources/DebugConfig.asset so it can be loaded at runtime.
    /// Implements the enableDebugLogs && categoryToggle pattern from Debugging Standard Section 7.
    /// </summary>
    [CreateAssetMenu(fileName = "DebugConfig", menuName = "Project/Debug Config")]
    public sealed class DebugConfig : ScriptableObject, IDebugConfig
    {
        // -------------------------------------------------------------------------
        // Master switch
        // -------------------------------------------------------------------------

        [SerializeField] private bool enableDebugLogs = true;

        // -------------------------------------------------------------------------
        // High Priority — Enabled by Default
        // -------------------------------------------------------------------------

        [Header("High Priority — Enabled by Default")]
        [SerializeField] private bool logGeneral = true;
        [SerializeField] private bool logBootstrap = true;
        [SerializeField] private bool logConfiguration = true;
        [SerializeField] private bool logValidation = true;
        [SerializeField] private bool logSaveLoad = true;
        [SerializeField] private bool logUI = true;
        [SerializeField] private bool logNavigation = true;

        // -------------------------------------------------------------------------
        // Verbose — Disabled by Default
        // -------------------------------------------------------------------------

        [Header("Verbose — Disabled by Default")]
        [SerializeField] private bool logDataDefinitions = false;
        [SerializeField] private bool logInput = false;
        [SerializeField] private bool logAudio = false;
        [SerializeField] private bool logVisual = false;
        [SerializeField] private bool logPerformance = false;
        [SerializeField] private bool logTesting = false;

        // -------------------------------------------------------------------------
        // IDebugConfig implementation
        // -------------------------------------------------------------------------

        /// <summary>
        /// Returns false if the master switch is off, or the specific category is disabled.
        /// </summary>
        public bool ShouldLog(DebugCategory category)
        {
            if (!enableDebugLogs)
            {
                return false;
            }

            return category switch
            {
                DebugCategory.General         => logGeneral,
                DebugCategory.Bootstrap       => logBootstrap,
                DebugCategory.Configuration   => logConfiguration,
                DebugCategory.Validation      => logValidation,
                DebugCategory.SaveLoad        => logSaveLoad,
                DebugCategory.UI              => logUI,
                DebugCategory.Navigation      => logNavigation,
                DebugCategory.DataDefinitions => logDataDefinitions,
                DebugCategory.Input           => logInput,
                DebugCategory.Audio           => logAudio,
                DebugCategory.Visual          => logVisual,
                DebugCategory.Performance     => logPerformance,
                DebugCategory.Testing         => logTesting,
                _                             => false
            };
        }
    }
}
