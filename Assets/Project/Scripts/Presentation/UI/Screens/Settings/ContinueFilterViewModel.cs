using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Settings
{
    /// <summary>
    /// Stable preset IDs for <see cref="ContinueFilterViewModel.SelectedPreset"/>.
    /// </summary>
    public static class ContinueFilterPresetIds
    {
        public const string CriticalOnly             = "critical_only";
        public const string ImportantAndCritical     = "important_and_critical";
        public const string AllReports               = "all_reports";
        public const string Custom                   = "custom";
    }

    /// <summary>
    /// Pure display-data class for the Continue interruption filter options in the Settings screen.
    /// Immutable after construction. No Unity dependencies.
    /// IsCustom indicates whether the custom category filter list is active.
    /// [Placeholder] — Phase 5 uses static data. Live wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ContinueFilterViewModel
    {
        /// <summary>
        /// Currently selected preset ID. See <see cref="ContinueFilterPresetIds"/>
        /// for allowed values: "critical_only", "important_and_critical", "all_reports", "custom".
        /// </summary>
        public string SelectedPreset { get; }

        /// <summary>
        /// Stable category IDs chosen when the preset is "custom".
        /// Empty when any non-custom preset is selected.
        /// </summary>
        public IReadOnlyList<string> CustomCategoryFilters { get; }

        /// <summary>True when SelectedPreset is "custom" and the custom filter list is active.</summary>
        public bool IsCustom { get; }

        public ContinueFilterViewModel(
            string selectedPreset,
            IReadOnlyList<string> customCategoryFilters,
            bool isCustom)
        {
            SelectedPreset        = selectedPreset;
            CustomCategoryFilters = customCategoryFilters;
            IsCustom              = isCustom;
        }
    }
}
