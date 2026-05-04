namespace Project.Presentation.UI.Screens.Settings
{
    /// <summary>
    /// Stable category tab IDs for the Settings screen.
    /// </summary>
    public static class SettingsCategoryIds
    {
        public const string General               = "cat.general";
        public const string Ui                    = "cat.ui";
        public const string Accessibility         = "cat.accessibility";
        public const string ContinueInterruptions = "cat.continue_interruptions";
        public const string SaveLoad              = "cat.save_load";
        public const string Audio                 = "cat.audio";
        public const string Controls              = "cat.controls";
    }

    /// <summary>
    /// Pure display-data class for one category tab in the Settings screen.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "active", "disabled".
    /// [Placeholder] — Phase 5 uses static data. Live wiring deferred to Phase 6+.
    /// </summary>
    public sealed class SettingsCategoryViewModel
    {
        /// <summary>Stable category ID, e.g. "cat.general". See <see cref="SettingsCategoryIds"/>.</summary>
        public string Id { get; }

        /// <summary>Display label for the tab, e.g. "General".</summary>
        public string Label { get; }

        /// <summary>Semantic state string: "normal", "active", or "disabled".</summary>
        public string SemanticState { get; }

        public SettingsCategoryViewModel(string id, string label, string semanticState)
        {
            Id            = id;
            Label         = label;
            SemanticState = semanticState;
        }
    }
}
