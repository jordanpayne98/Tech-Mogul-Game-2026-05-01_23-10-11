using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Products
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Products portfolio screen (screen.products).
    /// Immutable after construction. No Unity dependencies.
    /// Created by ProductsController and passed to ProductsView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductsViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Products".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there are no products to display.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there are no products to display.</summary>
        public string EmptyStateBody { get; }

        // ── Tab and toolbar state ─────────────────────────────────────────────

        /// <summary>
        /// Ordered list of tab IDs visible to the player.
        /// Valid values: "tab.all", "tab.in_development", "tab.ready_for_launch",
        /// "tab.launched", "tab.supported", "tab.cancelled_sunset".
        /// </summary>
        public IReadOnlyList<string> VisibleTabs { get; }

        /// <summary>Tab ID currently selected, matching one of the entries in <see cref="VisibleTabs"/>.</summary>
        public string ActiveTabId { get; }

        /// <summary>Current search bar text. Empty string when no search is active.</summary>
        public string SearchText { get; }

        /// <summary>Current filter drawer selection state.</summary>
        public ProductFilterViewModel FilterState { get; }

        // ── Product rows ──────────────────────────────────────────────────────

        /// <summary>Rows in the product portfolio table, pre-filtered and ordered for display.</summary>
        public IReadOnlyList<ProductRowViewModel> Rows { get; }

        /// <summary>
        /// Stable ID of the currently selected product, or empty string when nothing is selected.
        /// Used to keep the summary drawer in sync with the table selection.
        /// </summary>
        public string SelectedProductId { get; }

        // ── Derived visibility flags ──────────────────────────────────────────

        /// <summary>True when the product portfolio is empty after applying the active tab and filters.</summary>
        public bool HasNoProducts { get; }

        /// <summary>
        /// True when the player is permitted to open the Create Product entry point.
        /// [Placeholder] — always true in Phase 5.
        /// </summary>
        public bool CanCreateProduct { get; }

        public ProductsViewModel(
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
            ProductFilterViewModel filterState,
            IReadOnlyList<ProductRowViewModel> rows,
            string selectedProductId,
            bool hasNoProducts,
            bool canCreateProduct)
        {
            ScreenTitle       = screenTitle;
            ScreenSubtitle    = screenSubtitle;
            IsLoading         = isLoading;
            HasError          = hasError;
            ErrorMessage      = errorMessage;
            EmptyStateTitle   = emptyStateTitle;
            EmptyStateBody    = emptyStateBody;
            VisibleTabs       = visibleTabs;
            ActiveTabId       = activeTabId;
            SearchText        = searchText;
            FilterState       = filterState;
            Rows              = rows;
            SelectedProductId = selectedProductId;
            HasNoProducts     = hasNoProducts;
            CanCreateProduct  = canCreateProduct;
        }
    }
}
