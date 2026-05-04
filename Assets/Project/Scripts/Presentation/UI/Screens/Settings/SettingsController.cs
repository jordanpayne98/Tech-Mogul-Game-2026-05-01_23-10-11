using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;
using Project.Presentation.UI.Routing;

namespace Project.Presentation.UI.Screens.Settings
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Settings screen.
    /// Builds a [Placeholder] static ViewModel with 7 categories and 3-5 rows each.
    /// Handles category switching and placeholder Apply/Reset actions.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static data. Core settings wiring deferred to Phase 6+.
    /// </summary>
    public sealed class SettingsController
    {
        private readonly SettingsView  _view;
        private readonly IScreenRouter _screenRouter;
        private readonly IModalRouter  _modalRouter;

        // Tracks the currently active category ID for category switching.
        private string _activeCategoryId;

        // The full category list is built once and reused for ViewModel rebuilds.
        private IReadOnlyList<SettingsCategoryViewModel> _categories;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public SettingsController(
            SettingsView  view,
            IScreenRouter screenRouter,
            IModalRouter  modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnCategorySelected += HandleCategorySelected;
            _view.OnSettingChanged   += HandleSettingChanged;
            _view.OnApplyClicked     += HandleApplyClicked;
            _view.OnResetClicked     += HandleResetClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static data only. Core settings wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            _activeCategoryId = SettingsCategoryIds.General;
            _categories       = BuildCategories();

            SettingsScreenViewModel viewModel = BuildViewModel(_activeCategoryId);
            _view.Bind(viewModel);

            DebugLogger.Log(DebugCategory.UI,
                $"SettingsController: initialized. Active category: '{_activeCategoryId}'.");
        }

        // ─── Event handlers ───────────────────────────────────────────────────────────

        private void HandleCategorySelected(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return;
            }

            if (_activeCategoryId == categoryId)
            {
                return;
            }

            _activeCategoryId = categoryId;

            DebugLogger.Log(DebugCategory.UI,
                $"SettingsController: category selected — '{categoryId}'.");

            SettingsScreenViewModel viewModel = BuildViewModel(_activeCategoryId);
            _view.Bind(viewModel);
        }

        private void HandleSettingChanged(string settingId, string newValue)
        {
            // [Placeholder] — Phase 5 logs the change. Phase 6+ routes through a settings service.
            DebugLogger.Log(DebugCategory.UI,
                $"SettingsController: [Placeholder] setting changed — id='{settingId}' value='{newValue}'.");
        }

        private void HandleApplyClicked()
        {
            // [Placeholder] — Phase 5 logs only. Phase 6+ persists settings via a settings service.
            DebugLogger.Log(DebugCategory.UI,
                "SettingsController: [Placeholder] Apply clicked — settings persistence deferred to Phase 6+.");
        }

        private void HandleResetClicked()
        {
            // Destructive action — route through the confirmation modal.
            DebugLogger.Log(DebugCategory.UI,
                "SettingsController: [Placeholder] Reset to Defaults clicked — opening confirmation modal.");

            _modalRouter.OpenModal(ModalIds.Confirm);
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds the full Settings ViewModel for the given active category ID.
        /// Categories are rebuilt with updated SemanticState each time.
        /// [Placeholder] — All data is static. Replace with service calls in Phase 6+.
        /// </summary>
        private SettingsScreenViewModel BuildViewModel(string activeCategoryId)
        {
            IReadOnlyList<SettingsCategoryViewModel> categories = BuildCategoriesWithActiveState(activeCategoryId);
            IReadOnlyList<SettingRowViewModel>       rows       = BuildRowsForCategory(activeCategoryId);

            return new SettingsScreenViewModel(
                screenTitle:         "Settings",
                screenSubtitle:      string.Empty,
                isLoading:           false,
                hasError:            false,
                errorMessage:        string.Empty,
                emptyStateTitle:     "No Settings Available",
                emptyStateBody:      "No settings categories are available.",
                categories:          categories,
                activeCategoryId:    activeCategoryId,
                currentCategoryRows: rows,
                hasUnsavedChanges:   false,
                canApply:            false,
                canReset:            true);
        }

        private IReadOnlyList<SettingsCategoryViewModel> BuildCategories()
        {
            return new List<SettingsCategoryViewModel>
            {
                new SettingsCategoryViewModel(SettingsCategoryIds.General,               "General",                 "normal"),
                new SettingsCategoryViewModel(SettingsCategoryIds.Ui,                    "UI",                      "normal"),
                new SettingsCategoryViewModel(SettingsCategoryIds.Accessibility,         "Accessibility",           "normal"),
                new SettingsCategoryViewModel(SettingsCategoryIds.ContinueInterruptions, "Continue Interruptions",  "normal"),
                new SettingsCategoryViewModel(SettingsCategoryIds.SaveLoad,              "Save / Load",             "normal"),
                new SettingsCategoryViewModel(SettingsCategoryIds.Audio,                 "Audio [Placeholder]",     "normal"),
                new SettingsCategoryViewModel(SettingsCategoryIds.Controls,              "Controls [Placeholder]",  "normal"),
            };
        }

        private IReadOnlyList<SettingsCategoryViewModel> BuildCategoriesWithActiveState(string activeCategoryId)
        {
            var result = new List<SettingsCategoryViewModel>();

            foreach (SettingsCategoryViewModel cat in _categories)
            {
                string state = cat.Id == activeCategoryId ? "active" : "normal";
                result.Add(new SettingsCategoryViewModel(cat.Id, cat.Label, state));
            }

            return result;
        }

        /// <summary>
        /// Returns the setting rows for the requested category.
        /// All values are [Placeholder] static data.
        /// </summary>
        private static IReadOnlyList<SettingRowViewModel> BuildRowsForCategory(string categoryId)
        {
            switch (categoryId)
            {
                case SettingsCategoryIds.General:
                    return BuildGeneralRows();

                case SettingsCategoryIds.Ui:
                    return BuildUiRows();

                case SettingsCategoryIds.Accessibility:
                    return BuildAccessibilityRows();

                case SettingsCategoryIds.ContinueInterruptions:
                    return BuildContinueInterruptionRows();

                case SettingsCategoryIds.SaveLoad:
                    return BuildSaveLoadRows();

                case SettingsCategoryIds.Audio:
                    return BuildAudioRows();

                case SettingsCategoryIds.Controls:
                    return BuildControlsRows();

                default:
                    DebugLogger.LogWarning(DebugCategory.UI,
                        $"SettingsController.BuildRowsForCategory: unknown category ID '{categoryId}'. " +
                        "Returning empty row list.");
                    return new List<SettingRowViewModel>();
            }
        }

        // ─── Private — category row builders ─────────────────────────────────────────

        private static IReadOnlyList<SettingRowViewModel> BuildGeneralRows()
        {
            return new List<SettingRowViewModel>
            {
                new SettingRowViewModel(
                    id:            "setting.language",
                    label:         "Language",
                    description:   "Select the display language for all text.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "English",
                    options:       new[] { "English", "French", "German", "Spanish", "Japanese" },
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.autosave_enabled",
                    label:         "Autosave",
                    description:   "Automatically save your progress at regular intervals.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "true",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.pause_on_focus_loss",
                    label:         "Pause on Focus Loss",
                    description:   "Pause the simulation when the window loses focus.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "true",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.tooltips_enabled",
                    label:         "Show Tooltips",
                    description:   "Show contextual tooltips for UI elements.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "true",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.simulation_speed",
                    label:         "Default Simulation Speed",
                    description:   "Set the simulation speed used when starting a new game.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "Normal",
                    options:       new[] { "Slow", "Normal", "Fast" },
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),
            };
        }

        private static IReadOnlyList<SettingRowViewModel> BuildUiRows()
        {
            return new List<SettingRowViewModel>
            {
                new SettingRowViewModel(
                    id:            "setting.ui_density",
                    label:         "UI Density",
                    description:   "Adjust how compact the interface appears.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "Standard",
                    options:       new[] { "Compact", "Standard", "Spacious" },
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.show_sidebar_labels",
                    label:         "Show Sidebar Labels",
                    description:   "Display text labels below navigation icons.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "true",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.date_format",
                    label:         "Date Format",
                    description:   "Select how in-game dates are displayed.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "Day / Month / Year",
                    options:       new[] { "Day / Month / Year", "Month / Day / Year", "ISO (Year-Month-Day)" },
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.number_format",
                    label:         "Number Format",
                    description:   "Select how large numbers are displayed.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "1,000,000",
                    options:       new[] { "1,000,000", "1.000.000", "1M" },
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),
            };
        }

        private static IReadOnlyList<SettingRowViewModel> BuildAccessibilityRows()
        {
            return new List<SettingRowViewModel>
            {
                new SettingRowViewModel(
                    id:            "setting.text_scale",
                    label:         "Text Scale",
                    description:   "[Placeholder] Adjusts global text size. Full scaling deferred to Phase 6+.",
                    controlType:   SettingControlTypes.Slider,
                    currentValue:  "100",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.colourblind_labels",
                    label:         "Colourblind-Safe Labels",
                    description:   "Add text labels to colour-coded indicators.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "false",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.reduced_motion",
                    label:         "Reduced Motion",
                    description:   "[Placeholder] Reduces or disables transitions and animations.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "false",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.keyboard_focus_visible",
                    label:         "Keyboard Focus Highlight",
                    description:   "Show a visible highlight ring on focused UI elements.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "true",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.tooltip_delay",
                    label:         "Tooltip Delay",
                    description:   "[Placeholder] Delay before tooltips appear on hover.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "Normal",
                    options:       new[] { "Instant", "Short", "Normal", "Long" },
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),
            };
        }

        private static IReadOnlyList<SettingRowViewModel> BuildContinueInterruptionRows()
        {
            return new List<SettingRowViewModel>
            {
                new SettingRowViewModel(
                    id:            "setting.interruption_filter",
                    label:         "Interruption Filter",
                    description:   "Choose which events pause the simulation and require your decision.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "Important and Critical",
                    options:       new[] { "Critical only", "Important and Critical", "All reports", "Custom filters" },
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.pause_on_critical",
                    label:         "Pause on Critical Events",
                    description:   "Automatically pause the simulation for critical events.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "true",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.custom_filter_categories",
                    label:         "Custom Filter Categories",
                    description:   "[Placeholder] Configure which event categories trigger interruptions.",
                    controlType:   SettingControlTypes.Button,
                    currentValue:  "Configure...",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.interrupt_finance",
                    label:         "Finance Events",
                    description:   "Pause on significant financial events.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "true",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),

                new SettingRowViewModel(
                    id:            "setting.interrupt_hr",
                    label:         "HR / Hiring Events",
                    description:   "Pause on hiring offers, contract expirations, and employee departures.",
                    controlType:   SettingControlTypes.Toggle,
                    currentValue:  "true",
                    options:       new string[0],
                    isEnabled:     true,
                    isPlaceholder: false,
                    semanticState: "normal"),
            };
        }

        private static IReadOnlyList<SettingRowViewModel> BuildSaveLoadRows()
        {
            return new List<SettingRowViewModel>
            {
                new SettingRowViewModel(
                    id:            "setting.manual_save",
                    label:         "Manual Save Slots",
                    description:   "[Placeholder] Open the manual save slot browser.",
                    controlType:   SettingControlTypes.Button,
                    currentValue:  "Open Save Slots...",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.rolling_autosaves",
                    label:         "Rolling Autosaves",
                    description:   "[Placeholder] Configure rolling autosave slot count.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "5 slots",
                    options:       new[] { "3 slots", "5 slots", "10 slots" },
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.quick_save",
                    label:         "Quick Save",
                    description:   "[Placeholder] Quick save to the last-used slot.",
                    controlType:   SettingControlTypes.Button,
                    currentValue:  "Quick Save",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.named_company_saves",
                    label:         "Named Company Saves",
                    description:   "[Placeholder] Save and restore named company snapshots.",
                    controlType:   SettingControlTypes.Button,
                    currentValue:  "Manage Named Saves...",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.autosave_frequency",
                    label:         "Autosave Frequency",
                    description:   "[Placeholder] How often the game autosaves during play.",
                    controlType:   SettingControlTypes.Dropdown,
                    currentValue:  "Every 5 minutes",
                    options:       new[] { "Every 1 minute", "Every 5 minutes", "Every 15 minutes", "Off" },
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),
            };
        }

        private static IReadOnlyList<SettingRowViewModel> BuildAudioRows()
        {
            return new List<SettingRowViewModel>
            {
                new SettingRowViewModel(
                    id:            "setting.master_volume",
                    label:         "Master Volume [Placeholder]",
                    description:   "[Placeholder] Global audio volume. Full audio system deferred to Phase 6+.",
                    controlType:   SettingControlTypes.Slider,
                    currentValue:  "80",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.music_volume",
                    label:         "Music Volume [Placeholder]",
                    description:   "[Placeholder] Background music volume.",
                    controlType:   SettingControlTypes.Slider,
                    currentValue:  "70",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.sfx_volume",
                    label:         "SFX Volume [Placeholder]",
                    description:   "[Placeholder] Sound effects volume.",
                    controlType:   SettingControlTypes.Slider,
                    currentValue:  "90",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),
            };
        }

        private static IReadOnlyList<SettingRowViewModel> BuildControlsRows()
        {
            return new List<SettingRowViewModel>
            {
                new SettingRowViewModel(
                    id:            "setting.keybind_pause",
                    label:         "Pause / Unpause [Placeholder]",
                    description:   "[Placeholder] Key binding for pause. Full input remapping deferred to Phase 6+.",
                    controlType:   SettingControlTypes.Button,
                    currentValue:  "Space",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.keybind_speed_up",
                    label:         "Speed Up [Placeholder]",
                    description:   "[Placeholder] Key binding for increasing simulation speed.",
                    controlType:   SettingControlTypes.Button,
                    currentValue:  "]",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),

                new SettingRowViewModel(
                    id:            "setting.keybind_speed_down",
                    label:         "Speed Down [Placeholder]",
                    description:   "[Placeholder] Key binding for decreasing simulation speed.",
                    controlType:   SettingControlTypes.Button,
                    currentValue:  "[",
                    options:       new string[0],
                    isEnabled:     false,
                    isPlaceholder: true,
                    semanticState: "disabled"),
            };
        }
    }
}
