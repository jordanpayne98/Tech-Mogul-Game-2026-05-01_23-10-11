using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Contracts
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Contracts board screen (screen.contracts).
    /// Immutable after construction. No Unity dependencies.
    /// Created by ContractsController and passed to ContractsView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// Tab IDs: "tab.available", "tab.active", "tab.completed", "tab.failed_expired".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ContractsViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Contracts".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there is no content for the active tab.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there is no content for the active tab.</summary>
        public string EmptyStateBody { get; }

        // ── Tab and filter state ─────────────────────────────────────────────

        /// <summary>
        /// Ordered list of stable tab IDs visible in the tab bar.
        /// Accepted values: "tab.available", "tab.active", "tab.completed", "tab.failed_expired".
        /// </summary>
        public IReadOnlyList<string> VisibleTabs { get; }

        /// <summary>
        /// Stable ID of the currently selected tab.
        /// One of: "tab.available", "tab.active", "tab.completed", "tab.failed_expired".
        /// </summary>
        public string ActiveTabId { get; }

        /// <summary>Current search text entered in the toolbar search field. Empty string if none.</summary>
        public string SearchText { get; }

        /// <summary>Current state of the filter drawer selections.</summary>
        public ContractFilterViewModel FilterState { get; }

        // ── Contract list ────────────────────────────────────────────────────

        /// <summary>Ordered list of contract rows to display for the active tab, filtered and searched.</summary>
        public IReadOnlyList<ContractRowViewModel> Rows { get; }

        /// <summary>
        /// Stable ID of the currently selected contract row.
        /// Empty string when no contract is selected. Used to highlight the active row.
        /// </summary>
        public string SelectedContractId { get; }

        // ── Empty state flags ────────────────────────────────────────────────

        /// <summary>True when the "Available" tab has no contracts to display.</summary>
        public bool HasNoAvailableContracts { get; }

        /// <summary>True when the "Active" tab has no contracts to display.</summary>
        public bool HasNoActiveContracts { get; }

        public ContractsViewModel(
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
            ContractFilterViewModel filterState,
            IReadOnlyList<ContractRowViewModel> rows,
            string selectedContractId,
            bool hasNoAvailableContracts,
            bool hasNoActiveContracts)
        {
            ScreenTitle              = screenTitle;
            ScreenSubtitle           = screenSubtitle;
            IsLoading                = isLoading;
            HasError                 = hasError;
            ErrorMessage             = errorMessage;
            EmptyStateTitle          = emptyStateTitle;
            EmptyStateBody           = emptyStateBody;
            VisibleTabs              = visibleTabs;
            ActiveTabId              = activeTabId;
            SearchText               = searchText;
            FilterState              = filterState;
            Rows                     = rows;
            SelectedContractId       = selectedContractId;
            HasNoAvailableContracts  = hasNoAvailableContracts;
            HasNoActiveContracts     = hasNoActiveContracts;
        }
    }
}
