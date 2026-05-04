using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Competitors
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Competitors screen (screen.competitors).
    /// Immutable after construction. No Unity dependencies.
    /// Created by CompetitorsController and passed to CompetitorsView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompetitorsViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Competitors".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there are no competitors.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there are no competitors.</summary>
        public string EmptyStateBody { get; }

        // ── Search and filter ────────────────────────────────────────────────

        /// <summary>Current search query string entered by the user in the search toolbar.</summary>
        public string SearchText { get; }

        // ── Competitor table data ────────────────────────────────────────────

        /// <summary>Ordered list of competitor rows to display in the competitor table.</summary>
        public IReadOnlyList<CompetitorRowViewModel> Rows { get; }

        /// <summary>
        /// Stable ID of the currently selected competitor, or empty string if none selected.
        /// Used to drive detail drawer display and selection highlight state.
        /// </summary>
        public string SelectedCompetitorId { get; }

        // ── Derived visibility flags ─────────────────────────────────────────

        /// <summary>True when the Rows collection is empty and the screen should show empty state.</summary>
        public bool HasNoCompetitors { get; }

        public CompetitorsViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            string searchText,
            IReadOnlyList<CompetitorRowViewModel> rows,
            string selectedCompetitorId)
        {
            ScreenTitle          = screenTitle;
            ScreenSubtitle       = screenSubtitle;
            IsLoading            = isLoading;
            HasError             = hasError;
            ErrorMessage         = errorMessage;
            EmptyStateTitle      = emptyStateTitle;
            EmptyStateBody       = emptyStateBody;
            SearchText           = searchText;
            Rows                 = rows;
            SelectedCompetitorId = selectedCompetitorId;

            HasNoCompetitors = rows == null || rows.Count == 0;
        }
    }
}
