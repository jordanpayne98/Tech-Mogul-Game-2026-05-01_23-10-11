using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.ReportsInbox
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Reports / Inbox screen (screen.reports_inbox).
    /// Immutable after construction. No Unity dependencies.
    /// Created by ReportsInboxController and passed to ReportsInboxView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// Layout: three-panel inbox with category rail (left), report list (centre), detail panel (right).
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ReportsInboxViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Reports &amp; Inbox".</summary>
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

        // ── Category rail ────────────────────────────────────────────────────

        /// <summary>Current search filter text entered by the player. Empty string if no active search.</summary>
        public string SearchText { get; }

        /// <summary>All category entries displayed in the left rail.</summary>
        public IReadOnlyList<ReportCategoryViewModel> Categories { get; }

        /// <summary>Stable ID of the currently active category filter, e.g. "cat.all".</summary>
        public string ActiveCategoryId { get; }

        // ── Report list ──────────────────────────────────────────────────────

        /// <summary>Report rows displayed in the centre list panel for the active category and search filter.</summary>
        public IReadOnlyList<ReportRowViewModel> Reports { get; }

        /// <summary>Stable ID of the currently selected report. Empty string if nothing is selected.</summary>
        public string SelectedReportId { get; }

        // ── Detail panel ─────────────────────────────────────────────────────

        /// <summary>Detail data for the currently selected report. Null when no report is selected.</summary>
        public ReportDetailViewModel SelectedReportDetail { get; }

        // ── Summary states ───────────────────────────────────────────────────

        /// <summary>True when the inbox contains no reports at all (total inbox is empty).</summary>
        public bool HasEmptyInbox { get; }

        /// <summary>True when the active category or search filter returns no matching reports.</summary>
        public bool HasFilteredEmpty { get; }

        /// <summary>Total number of unread reports across all non-archived categories.</summary>
        public int UnreadCount { get; }

        /// <summary>Total number of reports that require a player decision.</summary>
        public int DecisionRequiredCount { get; }

        public ReportsInboxViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            string searchText,
            IReadOnlyList<ReportCategoryViewModel> categories,
            string activeCategoryId,
            IReadOnlyList<ReportRowViewModel> reports,
            string selectedReportId,
            ReportDetailViewModel selectedReportDetail,
            bool hasEmptyInbox,
            bool hasFilteredEmpty,
            int unreadCount,
            int decisionRequiredCount)
        {
            ScreenTitle           = screenTitle;
            ScreenSubtitle        = screenSubtitle;
            IsLoading             = isLoading;
            HasError              = hasError;
            ErrorMessage          = errorMessage;
            EmptyStateTitle       = emptyStateTitle;
            EmptyStateBody        = emptyStateBody;
            SearchText            = searchText;
            Categories            = categories;
            ActiveCategoryId      = activeCategoryId;
            Reports               = reports;
            SelectedReportId      = selectedReportId;
            SelectedReportDetail  = selectedReportDetail;
            HasEmptyInbox         = hasEmptyInbox;
            HasFilteredEmpty      = hasFilteredEmpty;
            UnreadCount           = unreadCount;
            DecisionRequiredCount = decisionRequiredCount;
        }
    }
}
