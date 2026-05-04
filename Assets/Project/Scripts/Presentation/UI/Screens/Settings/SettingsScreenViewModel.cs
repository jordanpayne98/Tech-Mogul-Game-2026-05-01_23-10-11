using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Settings
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Settings screen (screen.settings).
    /// Immutable after construction. No Unity dependencies.
    /// Created by SettingsScreenController and passed to SettingsScreenView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core settings wiring deferred to Phase 6+.
    /// </summary>
    public sealed class SettingsScreenViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Settings".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there is no content.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there is no content.</summary>
        public string EmptyStateBody { get; }

        // ── Category tabs ────────────────────────────────────────────────────

        /// <summary>All category tabs available in the Settings screen.</summary>
        public IReadOnlyList<SettingsCategoryViewModel> Categories { get; }

        /// <summary>Stable ID of the currently active category tab. See <see cref="SettingsCategoryIds"/>.</summary>
        public string ActiveCategoryId { get; }

        // ── Setting rows ─────────────────────────────────────────────────────

        /// <summary>Setting rows belonging to the currently active category.</summary>
        public IReadOnlyList<SettingRowViewModel> CurrentCategoryRows { get; }

        // ── Action bar state ─────────────────────────────────────────────────

        /// <summary>True when the user has changed one or more settings that have not yet been applied.</summary>
        public bool HasUnsavedChanges { get; }

        /// <summary>True when the Apply button should be enabled (HasUnsavedChanges and no blocking errors).</summary>
        public bool CanApply { get; }

        /// <summary>True when the Reset to Defaults button should be enabled.</summary>
        public bool CanReset { get; }

        public SettingsScreenViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            IReadOnlyList<SettingsCategoryViewModel> categories,
            string activeCategoryId,
            IReadOnlyList<SettingRowViewModel> currentCategoryRows,
            bool hasUnsavedChanges,
            bool canApply,
            bool canReset)
        {
            ScreenTitle          = screenTitle;
            ScreenSubtitle       = screenSubtitle;
            IsLoading            = isLoading;
            HasError             = hasError;
            ErrorMessage         = errorMessage;
            EmptyStateTitle      = emptyStateTitle;
            EmptyStateBody       = emptyStateBody;
            Categories           = categories;
            ActiveCategoryId     = activeCategoryId;
            CurrentCategoryRows  = currentCategoryRows;
            HasUnsavedChanges    = hasUnsavedChanges;
            CanApply             = canApply;
            CanReset             = canReset;
        }
    }
}
