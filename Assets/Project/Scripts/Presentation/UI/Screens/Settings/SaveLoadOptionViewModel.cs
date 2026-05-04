namespace Project.Presentation.UI.Screens.Settings
{
    /// <summary>
    /// Stable IDs for save/load option entries in the Settings screen.
    /// </summary>
    public static class SaveLoadOptionIds
    {
        public const string ManualSaveSlots    = "save.manual_slots";
        public const string RollingAutosaves   = "save.rolling_autosaves";
        public const string QuickSave          = "save.quick_save";
        public const string NamedCompanySaves  = "save.named_company_saves";
        public const string AutosaveFrequency  = "save.autosave_frequency";
    }

    /// <summary>
    /// Pure display-data class for one save/load option entry in the Settings screen.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "disabled".
    /// [Placeholder] — Phase 5 uses static data. Live save/load wiring deferred to Phase 6+.
    /// </summary>
    public sealed class SaveLoadOptionViewModel
    {
        /// <summary>Stable save/load option ID, e.g. "save.manual_slots". See <see cref="SaveLoadOptionIds"/>.</summary>
        public string Id { get; }

        /// <summary>Display label for the option, e.g. "Manual Save Slots".</summary>
        public string Label { get; }

        /// <summary>Optional description text shown beneath the label. Empty string if not shown.</summary>
        public string Description { get; }

        /// <summary>Pre-formatted current value string, e.g. "3 slots". Empty string if not applicable.</summary>
        public string CurrentValue { get; }

        /// <summary>True when the option is available and can be interacted with in the current state.</summary>
        public bool IsAvailable { get; }

        /// <summary>True when this entry carries placeholder content not yet backed by a real save/load system.</summary>
        public bool IsPlaceholder { get; }

        /// <summary>Semantic state string: "normal", "warning", or "disabled".</summary>
        public string SemanticState { get; }

        public SaveLoadOptionViewModel(
            string id,
            string label,
            string description,
            string currentValue,
            bool isAvailable,
            bool isPlaceholder,
            string semanticState)
        {
            Id            = id;
            Label         = label;
            Description   = description;
            CurrentValue  = currentValue;
            IsAvailable   = isAvailable;
            IsPlaceholder = isPlaceholder;
            SemanticState = semanticState;
        }
    }
}
