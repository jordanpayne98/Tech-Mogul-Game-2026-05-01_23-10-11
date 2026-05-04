using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Settings
{
    /// <summary>
    /// Stable control type strings for <see cref="SettingRowViewModel.ControlType"/>.
    /// </summary>
    public static class SettingControlTypes
    {
        public const string Toggle   = "toggle";
        public const string Dropdown = "dropdown";
        public const string Slider   = "slider";
        public const string Button   = "button";
    }

    /// <summary>
    /// Pure display-data class for one setting row in the Settings screen.
    /// Immutable after construction. No Unity dependencies.
    /// ControlType determines which UI control the View renders ("toggle", "dropdown", "slider", "button").
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "disabled".
    /// [Placeholder] — Phase 5 uses static data. Live wiring deferred to Phase 6+.
    /// </summary>
    public sealed class SettingRowViewModel
    {
        /// <summary>Stable setting ID, e.g. "setting.audio_volume".</summary>
        public string Id { get; }

        /// <summary>Display label for the setting row, e.g. "Master Volume".</summary>
        public string Label { get; }

        /// <summary>Optional description text shown beneath the label. Empty string if not shown.</summary>
        public string Description { get; }

        /// <summary>
        /// Control type identifier. See <see cref="SettingControlTypes"/>
        /// for the allowed values: "toggle", "dropdown", "slider", "button".
        /// </summary>
        public string ControlType { get; }

        /// <summary>Pre-formatted current value string, e.g. "On", "English", "75%". Empty string if not applicable.</summary>
        public string CurrentValue { get; }

        /// <summary>Available options for dropdown controls. Empty for toggles, sliders, and buttons.</summary>
        public IReadOnlyList<string> Options { get; }

        /// <summary>True when the control is interactive. False when the setting is locked or unavailable.</summary>
        public bool IsEnabled { get; }

        /// <summary>True when this row carries placeholder content not yet backed by a real setting.</summary>
        public bool IsPlaceholder { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "disabled".</summary>
        public string SemanticState { get; }

        public SettingRowViewModel(
            string id,
            string label,
            string description,
            string controlType,
            string currentValue,
            IReadOnlyList<string> options,
            bool isEnabled,
            bool isPlaceholder,
            string semanticState)
        {
            Id            = id;
            Label         = label;
            Description   = description;
            ControlType   = controlType;
            CurrentValue  = currentValue;
            Options       = options;
            IsEnabled     = isEnabled;
            IsPlaceholder = isPlaceholder;
            SemanticState = semanticState;
        }
    }
}
