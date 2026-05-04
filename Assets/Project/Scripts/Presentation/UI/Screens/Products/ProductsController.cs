using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Products
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Products portfolio screen.
    /// Builds a [Placeholder] static ViewModel with 3–5 products across multiple types and statuses,
    /// handles tab, row, create-product, and filter callbacks, and binds the ViewModel to the View.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductsController
    {
        private readonly ProductsView  _view;
        private readonly IScreenRouter _screenRouter;
        private readonly IModalRouter  _modalRouter;

        // ─── State ───────────────────────────────────────────────────────────────────

        // Tracks the currently active tab so Bind can reflect selection state.
        private string _activeTabId = "tab.all";

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public ProductsController(
            ProductsView  view,
            IScreenRouter screenRouter,
            IModalRouter  modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnTabSelected          += HandleTabSelected;
            _view.OnProductRowClicked    += HandleProductRowClicked;
            _view.OnCreateProductClicked += HandleCreateProductClicked;
            _view.OnFiltersButtonClicked += HandleFiltersButtonClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            ProductsViewModel viewModel = BuildPlaceholderViewModel(_activeTabId);
            _view.Bind(viewModel);

            DebugLogger.Log(DebugCategory.UI,
                "ProductsController.Initialize: [Placeholder] ViewModel bound.");
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleTabSelected(string tabId)
        {
            if (string.IsNullOrEmpty(tabId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"ProductsController: tab selected — '{tabId}'.");

            _activeTabId = tabId;

            // Rebuild the ViewModel with the newly active tab and re-bind.
            // Phase 5: tab selection simply updates the active state; real filtering deferred to Phase 6+.
            ProductsViewModel viewModel = BuildPlaceholderViewModel(_activeTabId);
            _view.Bind(viewModel);
        }

        private void HandleProductRowClicked(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"ProductsController: product row clicked — productId '{productId}'. " +
                $"[Placeholder] Navigating to '{ScreenIds.ProductDetail}'.");

            // Phase 5: navigate to the Product Detail screen.
            // Phase 6+ will pass the product ID as context via the router.
            _screenRouter.OpenScreen(ScreenIds.ProductDetail);
        }

        private void HandleCreateProductClicked()
        {
            DebugLogger.Log(DebugCategory.UI,
                "ProductsController: Create Product clicked. [Placeholder] Modal will be opened in a later phase.");

            // Phase 5 placeholder — opens the generic info modal.
            _modalRouter.OpenModal(ModalIds.Info);
        }

        private void HandleFiltersButtonClicked()
        {
            DebugLogger.Log(DebugCategory.UI,
                "ProductsController: Filters button clicked. [Placeholder] Filter drawer will be wired in a later phase.");

            // Phase 5 placeholder — opens the generic filter drawer placeholder.
            // A real filter drawer specific to Products will be wired in a later phase.
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] ProductsViewModel for Phase 5.
        /// Contains 5 products spanning multiple families, types, and lifecycle statuses.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// </summary>
        private static ProductsViewModel BuildPlaceholderViewModel(string activeTabId)
        {
            var rows = BuildPlaceholderRows();

            var visibleTabs = new List<string>
            {
                "tab.all",
                "tab.in_development",
                "tab.ready_for_launch",
                "tab.launched",
                "tab.supported",
                "tab.cancelled_sunset",
            };

            var filterState = new ProductFilterViewModel(
                familyFilter:      string.Empty,
                typeFilter:        string.Empty,
                statusFilter:      string.Empty,
                teamFilter:        string.Empty,
                hasActiveFilters:  false,
                activeFilterCount: 0);

            return new ProductsViewModel(
                screenTitle:       "Products",
                screenSubtitle:    "Manage your product portfolio, status, and performance. [Placeholder]",
                isLoading:         false,
                hasError:          false,
                errorMessage:      string.Empty,
                emptyStateTitle:   "No Products Yet",
                emptyStateBody:    "Create your first product to start building your portfolio.",
                visibleTabs:       visibleTabs,
                activeTabId:       activeTabId,
                searchText:        string.Empty,
                filterState:       filterState,
                rows:              rows,
                selectedProductId: string.Empty,
                hasNoProducts:     false,
                canCreateProduct:  true);
        }

        /// <summary>
        /// Builds a static list of 5 [Placeholder] product rows.
        /// Products cover: Web Platform (in development), Business SaaS (launched),
        /// Peripheral (supported), Business SaaS (ready for launch), Web Platform (cancelled).
        /// </summary>
        private static IReadOnlyList<ProductRowViewModel> BuildPlaceholderRows()
        {
            return new List<ProductRowViewModel>
            {
                // ── Product 1: In Development — Web Platform ─────────────────────────
                new ProductRowViewModel(
                    id:                 "product.alpha_web_platform",
                    productName:        "StreamFlow [Placeholder]",
                    family:             "Software",
                    type:               "Web Platform",
                    status:             "In Development",
                    assignedTeam:       "Prototype Team [Placeholder]",
                    phase:              "Pre-Alpha",
                    releaseTarget:      "Q3 Y2",
                    reviewScore:        "—",
                    recentReviewScore:  "—",
                    revenueThisMonth:   "—",
                    activeUsersOrUnits: "—",
                    supportLoad:        "—",
                    semanticState:      "neutral",
                    isClickable:        true,
                    drillDownRouteId:   ScreenIds.ProductDetail),

                // ── Product 2: Launched — Business SaaS ──────────────────────────────
                new ProductRowViewModel(
                    id:                 "product.beta_business_saas",
                    productName:        "OfficeSync Pro [Placeholder]",
                    family:             "Software",
                    type:               "Business SaaS",
                    status:             "Launched",
                    assignedTeam:       "Core Products Team [Placeholder]",
                    phase:              "v1.4",
                    releaseTarget:      "—",
                    reviewScore:        "82",
                    recentReviewScore:  "79",
                    revenueThisMonth:   "$12,400",
                    activeUsersOrUnits: "3,200",
                    supportLoad:        "Moderate",
                    semanticState:      "positive",
                    isClickable:        true,
                    drillDownRouteId:   ScreenIds.ProductDetail),

                // ── Product 3: Supported — Peripheral ────────────────────────────────
                new ProductRowViewModel(
                    id:                 "product.gamma_peripheral",
                    productName:        "TactilePad v2 [Placeholder]",
                    family:             "Hardware",
                    type:               "Peripheral",
                    status:             "Supported",
                    assignedTeam:       "Hardware Team [Placeholder]",
                    phase:              "Support v2.1",
                    releaseTarget:      "—",
                    reviewScore:        "74",
                    recentReviewScore:  "71",
                    revenueThisMonth:   "$4,800",
                    activeUsersOrUnits: "8,500",
                    supportLoad:        "Low",
                    semanticState:      "neutral",
                    isClickable:        true,
                    drillDownRouteId:   ScreenIds.ProductDetail),

                // ── Product 4: Ready for Launch — Business SaaS ──────────────────────
                new ProductRowViewModel(
                    id:                 "product.delta_ready_saas",
                    productName:        "DataPulse Analytics [Placeholder]",
                    family:             "Software",
                    type:               "Business SaaS",
                    status:             "Ready for Launch",
                    assignedTeam:       "Growth Team [Placeholder]",
                    phase:              "RC1",
                    releaseTarget:      "Q1 Y3",
                    reviewScore:        "—",
                    recentReviewScore:  "—",
                    revenueThisMonth:   "—",
                    activeUsersOrUnits: "—",
                    supportLoad:        "—",
                    semanticState:      "warning",
                    isClickable:        true,
                    drillDownRouteId:   ScreenIds.ProductDetail),

                // ── Product 5: Cancelled — Web Platform ──────────────────────────────
                new ProductRowViewModel(
                    id:                 "product.epsilon_cancelled_web",
                    productName:        "NetGrid [Placeholder]",
                    family:             "Software",
                    type:               "Web Platform",
                    status:             "Cancelled",
                    assignedTeam:       "—",
                    phase:              "—",
                    releaseTarget:      "—",
                    reviewScore:        "—",
                    recentReviewScore:  "—",
                    revenueThisMonth:   "—",
                    activeUsersOrUnits: "—",
                    supportLoad:        "—",
                    semanticState:      "muted",
                    isClickable:        true,
                    drillDownRouteId:   ScreenIds.ProductDetail),
            };
        }
    }
}
