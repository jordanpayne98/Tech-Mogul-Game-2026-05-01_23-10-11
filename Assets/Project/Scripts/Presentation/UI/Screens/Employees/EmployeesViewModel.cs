using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Employees
{
    /// <summary>
    /// Tab IDs used in the Employees roster screen tab bar.
    /// </summary>
    public static class EmployeesTabIds
    {
        public const string All        = "tab.all";
        public const string ByTeam     = "tab.by_team";
        public const string Unassigned = "tab.unassigned";
        public const string AtRisk     = "tab.at_risk";
        public const string Former     = "tab.former";
    }

    /// <summary>
    /// Top-level aggregate ViewModel for the Employees roster screen (screen.employees).
    /// Immutable after construction. No Unity dependencies.
    /// Created by EmployeesController and passed to EmployeesView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class EmployeesViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Employees".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there are no employees.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there are no employees.</summary>
        public string EmptyStateBody { get; }

        // ── Tab and search state ─────────────────────────────────────────────

        /// <summary>Ordered list of stable tab IDs visible in the tab bar.</summary>
        public IReadOnlyList<string> VisibleTabs { get; }

        /// <summary>The currently active tab ID, e.g. "tab.all".</summary>
        public string ActiveTabId { get; }

        /// <summary>Current search box text. Empty string if no search is active.</summary>
        public string SearchText { get; }

        /// <summary>Current filter drawer display state.</summary>
        public EmployeeFilterViewModel FilterState { get; }

        // ── Employee roster ──────────────────────────────────────────────────

        /// <summary>Employee rows to display in the roster table, filtered and sorted by the Controller.</summary>
        public IReadOnlyList<EmployeeRowViewModel> Rows { get; }

        /// <summary>Stable ID of the currently selected employee row. Empty string if none selected.</summary>
        public string SelectedEmployeeId { get; }

        // ── Empty / recruitment state ────────────────────────────────────────

        /// <summary>True when there are no employees to display (after filtering).</summary>
        public bool HasNoEmployees { get; }

        /// <summary>Route ID used to navigate to the recruitment screen, e.g. "screen.recruitment".</summary>
        public string RecruitmentRouteId { get; }

        public EmployeesViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            IReadOnlyList<string> visibleTabs,
            string activeTabId,
            string searchText,
            EmployeeFilterViewModel filterState,
            IReadOnlyList<EmployeeRowViewModel> rows,
            string selectedEmployeeId,
            bool hasNoEmployees,
            string recruitmentRouteId)
        {
            ScreenTitle         = screenTitle;
            ScreenSubtitle      = screenSubtitle;
            IsLoading           = isLoading;
            HasError            = hasError;
            ErrorMessage        = errorMessage;
            EmptyStateTitle     = emptyStateTitle;
            EmptyStateBody      = emptyStateBody;
            VisibleTabs         = visibleTabs;
            ActiveTabId         = activeTabId;
            SearchText          = searchText;
            FilterState         = filterState;
            Rows                = rows;
            SelectedEmployeeId  = selectedEmployeeId;
            HasNoEmployees      = hasNoEmployees;
            RecruitmentRouteId  = recruitmentRouteId;
        }
    }
}
