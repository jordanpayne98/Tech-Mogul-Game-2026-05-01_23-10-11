using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.ProductDetail
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Product Detail screen.
    /// Builds a [Placeholder] static ViewModel for one product, wires tab-switching and
    /// back-navigation callbacks, and calls View.Bind() to display data.
    /// Must not own core rules, save/load, or persistent state.
    /// Launch, Delay, and Cancel actions are disabled in Phase 5 — lifecycle state changes
    /// belong to Application/Core and will be wired in a later phase.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductDetailController
    {
        private readonly ProductDetailView _view;
        private readonly IScreenRouter     _screenRouter;

        // Tracks the active tab ID so re-binds preserve tab state.
        private string _activeTabId;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view click events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public ProductDetailController(
            ProductDetailView view,
            IScreenRouter     screenRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _activeTabId  = ProductDetailTabIds.Overview;

            _view.OnTabSelected      += HandleTabSelected;
            _view.OnBackClicked      += HandleBackClicked;
            _view.OnMetricCardClicked += HandleMetricCardClicked;

            // Launch, Delay, Cancel are wired but blocked in Phase 5 (buttons are disabled).
            _view.OnLaunchClicked += HandleLaunchClicked;
            _view.OnDelayClicked  += HandleDelayClicked;
            _view.OnCancelClicked += HandleCancelClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            BindViewModel();

            DebugLogger.Log(DebugCategory.UI,
                "ProductDetailController: initialized with [Placeholder] static ViewModel.");
        }

        // ─── Private — bind ───────────────────────────────────────────────────────────

        private void BindViewModel()
        {
            ProductDetailScreenViewModel viewModel = BuildPlaceholderViewModel(_activeTabId);
            _view.Bind(viewModel);
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleTabSelected(string tabId)
        {
            if (string.IsNullOrEmpty(tabId))
            {
                return;
            }

            if (_activeTabId == tabId)
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"ProductDetailController: tab selected — '{tabId}'.");

            _activeTabId = tabId;
            BindViewModel();
        }

        private void HandleBackClicked()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                $"ProductDetailController: back clicked — navigating to '{ScreenIds.Products}'.");

            _screenRouter.OpenScreen(ScreenIds.Products);
        }

        private void HandleMetricCardClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"ProductDetailController: metric card clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        private void HandleLaunchClicked()
        {
            // Phase 5: Launch action is disabled. Log only for traceability.
            DebugLogger.Log(DebugCategory.UI,
                "[Placeholder] ProductDetailController: Launch clicked — lifecycle wiring deferred to Phase 6.");
        }

        private void HandleDelayClicked()
        {
            // Phase 5: Delay action is disabled. Log only for traceability.
            DebugLogger.Log(DebugCategory.UI,
                "[Placeholder] ProductDetailController: Delay clicked — lifecycle wiring deferred to Phase 6.");
        }

        private void HandleCancelClicked()
        {
            // Phase 5: Cancel action is disabled. Log only for traceability.
            DebugLogger.Log(DebugCategory.UI,
                "[Placeholder] ProductDetailController: Cancel clicked — lifecycle wiring deferred to Phase 6.");
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] ProductDetailScreenViewModel for Phase 5.
        /// Product: "TechOS Pro" — Business SaaS — In Development.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// </summary>
        private static ProductDetailScreenViewModel BuildPlaceholderViewModel(string activeTabId)
        {
            return new ProductDetailScreenViewModel(
                screenTitle:      "TechOS Pro",
                screenSubtitle:   "Product Detail [Placeholder]",
                isLoading:        false,
                hasError:         false,
                errorMessage:     string.Empty,
                emptyStateTitle:  "No Product Found",
                emptyStateBody:   "The requested product does not exist or could not be loaded.",
                productId:        "product.techos_pro",
                productName:      "TechOS Pro",
                family:           "Business Software",
                type:             "SaaS Platform",
                status:           "In Development",
                targetMarket:     "Enterprise",
                customerSegment:  "SMB",
                priceModel:       "Subscription",
                launchTarget:     "Q3, Year 2 [Placeholder]",
                visibleTabs:      BuildAllTabs(),
                activeTabId:      activeTabId,
                overviewCards:    BuildPlaceholderOverviewCards(),
                riskSummary:      BuildPlaceholderRiskSummary(),
                historyRows:      BuildPlaceholderHistoryRows(),
                canLaunch:        false,
                canDelay:         false,
                canCancel:        false,
                backRouteId:      ScreenIds.Products,
                hasNoAssignedTeam: false,
                hasHighRisk:      false,
                semanticState:    "normal");
        }

        private static IReadOnlyList<string> BuildAllTabs()
        {
            return new List<string>
            {
                ProductDetailTabIds.Overview,
                ProductDetailTabIds.Development,
                ProductDetailTabIds.Quality,
                ProductDetailTabIds.Budget,
                ProductDetailTabIds.Marketing,
                ProductDetailTabIds.Support,
                ProductDetailTabIds.Reports,
                ProductDetailTabIds.History,
                ProductDetailTabIds.Competitors,
            };
        }

        private static IReadOnlyList<ProductMetricCardViewModel> BuildPlaceholderOverviewCards()
        {
            return new List<ProductMetricCardViewModel>
            {
                new ProductMetricCardViewModel(
                    id:               "metric.current_phase",
                    label:            "Current Phase",
                    value:            "Development",
                    trendText:        "[Placeholder] Phase 3 of 5",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new ProductMetricCardViewModel(
                    id:               "metric.assigned_teams",
                    label:            "Assigned Teams",
                    value:            "1 Team",
                    trendText:        "[Placeholder] Core Dev Squad",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Teams,
                    isClickable:      true),

                new ProductMetricCardViewModel(
                    id:               "metric.progress",
                    label:            "Progress",
                    value:            "42%",
                    trendText:        "[Placeholder] +8% this month",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new ProductMetricCardViewModel(
                    id:               "metric.revenue",
                    label:            "Revenue / Users",
                    value:            "$0 / 0 users",
                    trendText:        "[Placeholder] Pre-launch",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Finance,
                    isClickable:      true),

                new ProductMetricCardViewModel(
                    id:               "metric.support_load",
                    label:            "Support Load",
                    value:            "None",
                    trendText:        "[Placeholder] Pre-launch",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new ProductMetricCardViewModel(
                    id:               "metric.market_position",
                    label:            "Market Position",
                    value:            "Not Launched",
                    trendText:        "[Placeholder] Market data pending",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Market,
                    isClickable:      true),
            };
        }

        private static ProductRiskSummaryViewModel BuildPlaceholderRiskSummary()
        {
            return new ProductRiskSummaryViewModel(
                overallScore:              "68 / 100 [Placeholder]",
                quality:                   "Medium",
                creativity:                "High",
                stability:                 "Medium",
                bugRisk:                   "Low",
                qaConfidence:              "72%",
                infrastructureReadiness:   "Partial",
                supportReadiness:          "Not Ready",
                developmentBudget:         "$120,000 / mo [Placeholder]",
                preLaunchMarketingBudget:  "$25,000 / mo [Placeholder]",
                postLaunchMarketingBudget: "$15,000 / mo [Placeholder]",
                postLaunchSupportBudget:   "$8,000 / mo [Placeholder]",
                semanticState:             "normal");
        }

        private static IReadOnlyList<ProductHistoryRowViewModel> BuildPlaceholderHistoryRows()
        {
            return new List<ProductHistoryRowViewModel>
            {
                new ProductHistoryRowViewModel(
                    date:          "Month 1, Year 1",
                    @event:        "[Placeholder] Product definition created — TechOS Pro",
                    category:      "Lifecycle",
                    semanticState: "normal"),

                new ProductHistoryRowViewModel(
                    date:          "Month 2, Year 1",
                    @event:        "[Placeholder] Core Dev Squad assigned",
                    category:      "Team",
                    semanticState: "normal"),

                new ProductHistoryRowViewModel(
                    date:          "Month 3, Year 1",
                    @event:        "[Placeholder] Development phase started — architecture review complete",
                    category:      "Development",
                    semanticState: "normal"),

                new ProductHistoryRowViewModel(
                    date:          "Month 4, Year 1",
                    @event:        "[Placeholder] Quality review — stability flagged at medium risk",
                    category:      "Quality",
                    semanticState: "warning"),

                new ProductHistoryRowViewModel(
                    date:          "Month 5, Year 1",
                    @event:        "[Placeholder] Budget increased — additional infra allocated",
                    category:      "Budget",
                    semanticState: "normal"),
            };
        }
    }
}
