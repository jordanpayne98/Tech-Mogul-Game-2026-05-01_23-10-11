using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.FounderPortal
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Founder Portal screen.
    /// Builds a [Placeholder] static ViewModel, wires click callbacks to ScreenRouter/ModalRouter,
    /// and calls View.Bind() to display data.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class FounderPortalController
    {
        private readonly FounderPortalView _view;
        private readonly IScreenRouter     _screenRouter;
        private readonly IModalRouter      _modalRouter;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view click events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public FounderPortalController(
            FounderPortalView view,
            IScreenRouter     screenRouter,
            IModalRouter      modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnKpiCardClicked        += HandleKpiCardClicked;
            _view.OnDashboardCardClicked  += HandleDashboardCardClicked;
            _view.OnQuickActionClicked    += HandleQuickActionClicked;
            _view.OnRecentActivityClicked += HandleRecentActivityClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            FounderPortalViewModel viewModel = BuildPlaceholderViewModel();
            _view.Bind(viewModel);
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleKpiCardClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"FounderPortalController: KPI card clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        private void HandleDashboardCardClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"FounderPortalController: dashboard card clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        private void HandleQuickActionClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"FounderPortalController: quick action clicked — routing to '{routeId}'.");

            // All Phase 5 quick actions route to placeholder screens via the screen router.
            // Phase 6+ may route some actions to modal-based flows; check route prefix then.
            _screenRouter.OpenScreen(routeId);
        }

        private void HandleRecentActivityClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"FounderPortalController: activity item clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] FounderPortalViewModel for Phase 5.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// </summary>
        private static FounderPortalViewModel BuildPlaceholderViewModel()
        {
            return new FounderPortalViewModel(
                screenTitle:           "Founder Portal",
                screenSubtitle:        "Company Overview [Placeholder]",
                isLoading:             false,
                hasError:              false,
                errorMessage:          string.Empty,
                emptyStateTitle:       "No Data Available",
                emptyStateBody:        "Start a new company to see your dashboard.",
                kpiCards:              BuildPlaceholderKpiCards(),
                dashboardCards:        BuildPlaceholderDashboardCards(),
                recentActivityItems:   BuildPlaceholderRecentActivity(),
                quickActions:          BuildPlaceholderQuickActions(),
                upNextItem:            BuildPlaceholderUpNext(),
                hasLowRunwayWarning:   false,
                hasHighWorkloadWarning: false,
                hasUnreadDecisions:    false,
                hasInfrastructureRisk: false);
        }

        private static IReadOnlyList<KpiCardViewModel> BuildPlaceholderKpiCards()
        {
            return new List<KpiCardViewModel>
            {
                new KpiCardViewModel(
                    id:               KpiCardIds.Cash,
                    label:            "Cash / Available Funds",
                    value:            "$1,250,000",
                    trendText:        "+12% this month",
                    semanticState:    "success",
                    drillDownRouteId: ScreenIds.Finance,
                    isClickable:      true),

                new KpiCardViewModel(
                    id:               KpiCardIds.BurnRate,
                    label:            "Burn Rate",
                    value:            "-$85,000/mo",
                    trendText:        "Steady",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Finance,
                    isClickable:      true),

                new KpiCardViewModel(
                    id:               KpiCardIds.Runway,
                    label:            "Runway",
                    value:            "14.7 months",
                    trendText:        "Based on current burn rate",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Finance,
                    isClickable:      true),

                new KpiCardViewModel(
                    id:               KpiCardIds.RevenueMtd,
                    label:            "Revenue MTD",
                    value:            "$42,500",
                    trendText:        "+8% vs last month",
                    semanticState:    "success",
                    drillDownRouteId: ScreenIds.Reports,
                    isClickable:      true),

                new KpiCardViewModel(
                    id:               KpiCardIds.ActiveAlerts,
                    label:            "Active Alerts",
                    value:            "3",
                    trendText:        "2 new since last visit",
                    semanticState:    "warning",
                    drillDownRouteId: ScreenIds.Portal,
                    isClickable:      false),

                new KpiCardViewModel(
                    id:               KpiCardIds.ActiveProducts,
                    label:            "Active Products",
                    value:            "2",
                    trendText:        "In development",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Products,
                    isClickable:      true),
            };
        }

        private static IReadOnlyList<DashboardCardViewModel> BuildPlaceholderDashboardCards()
        {
            return new List<DashboardCardViewModel>
            {
                new DashboardCardViewModel(
                    id:               "card.inbox",
                    title:            "Inbox / Requires Decision",
                    summaryText:      "3 items pending",
                    semanticState:    "warning",
                    drillDownRouteId: ScreenIds.Portal,
                    isClickable:      false,
                    detailLines:      new[] { "Contract renewal due", "Hiring offer pending", "Infrastructure alert" }),

                new DashboardCardViewModel(
                    id:               "card.products",
                    title:            "Products",
                    summaryText:      "2 active products",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Products,
                    isClickable:      true,
                    detailLines:      new[] { "Alpha App — in development", "Beta Service — in review" }),

                new DashboardCardViewModel(
                    id:               "card.team",
                    title:            "Team",
                    summaryText:      "8 employees",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Employees,
                    isClickable:      true,
                    detailLines:      new[] { "3 developers", "2 designers", "3 operations" }),

                new DashboardCardViewModel(
                    id:               "card.hiring",
                    title:            "Hiring",
                    summaryText:      "1 open role",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Recruitment,
                    isClickable:      true,
                    detailLines:      new[] { "Senior Developer — interview stage" }),

                new DashboardCardViewModel(
                    id:               "card.milestones",
                    title:            "Milestones",
                    summaryText:      "Next: MVP Launch",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Reports,
                    isClickable:      true,
                    detailLines:      new[] { "[Placeholder] Day 45 target" }),

                new DashboardCardViewModel(
                    id:               "card.market",
                    title:            "Market",
                    summaryText:      "2 competitors tracked",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Market,
                    isClickable:      true,
                    detailLines:      new[] { "[Placeholder] Market data pending" }),

                new DashboardCardViewModel(
                    id:               "card.infrastructure",
                    title:            "Infrastructure",
                    summaryText:      "All systems operational",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Infrastructure,
                    isClickable:      true,
                    detailLines:      new[] { "[Placeholder] No incidents reported" }),
            };
        }

        private static IReadOnlyList<QuickActionViewModel> BuildPlaceholderQuickActions()
        {
            return new List<QuickActionViewModel>
            {
                new QuickActionViewModel(
                    id:            QuickActionIds.NewProduct,
                    label:         "New Product",
                    iconClass:     "icon-product",
                    targetRouteId: ScreenIds.Products,
                    isEnabled:     true),

                new QuickActionViewModel(
                    id:            QuickActionIds.HireEmployee,
                    label:         "Hire Employee",
                    iconClass:     "icon-employee",
                    targetRouteId: ScreenIds.Recruitment,
                    isEnabled:     true),

                new QuickActionViewModel(
                    id:            QuickActionIds.CreateContract,
                    label:         "Create Contract",
                    iconClass:     "icon-contract",
                    targetRouteId: ScreenIds.Contracts,
                    isEnabled:     true),

                new QuickActionViewModel(
                    id:            QuickActionIds.ViewReports,
                    label:         "View Reports",
                    iconClass:     "icon-report",
                    targetRouteId: ScreenIds.Reports,
                    isEnabled:     true),

                new QuickActionViewModel(
                    id:            QuickActionIds.MarketOverview,
                    label:         "Market Overview",
                    iconClass:     "icon-market",
                    targetRouteId: ScreenIds.Market,
                    isEnabled:     true),

                new QuickActionViewModel(
                    id:            QuickActionIds.Settings,
                    label:         "Settings",
                    iconClass:     "icon-settings",
                    targetRouteId: ScreenIds.Settings,
                    isEnabled:     true),
            };
        }

        private static IReadOnlyList<RecentActivityItemViewModel> BuildPlaceholderRecentActivity()
        {
            return new List<RecentActivityItemViewModel>
            {
                new RecentActivityItemViewModel(
                    id:               "activity.001",
                    timestamp:        "Day 14, 09:00",
                    description:      "[Placeholder] Hired Alex Chen — Senior Developer",
                    categoryLabel:    "Hiring",
                    semanticState:    "info",
                    drillDownRouteId: ScreenIds.Employees,
                    isClickable:      true),

                new RecentActivityItemViewModel(
                    id:               "activity.002",
                    timestamp:        "Day 13, 14:30",
                    description:      "[Placeholder] Product 'Alpha App' entered code review",
                    categoryLabel:    "Product",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Products,
                    isClickable:      true),

                new RecentActivityItemViewModel(
                    id:               "activity.003",
                    timestamp:        "Day 13, 08:00",
                    description:      "[Placeholder] Monthly burn rate updated — $85,000",
                    categoryLabel:    "Finance",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Finance,
                    isClickable:      true),

                new RecentActivityItemViewModel(
                    id:               "activity.004",
                    timestamp:        "Day 12, 16:00",
                    description:      "[Placeholder] Infrastructure alert: server capacity at 85%",
                    categoryLabel:    "Infrastructure",
                    semanticState:    "warning",
                    drillDownRouteId: ScreenIds.Infrastructure,
                    isClickable:      true),

                new RecentActivityItemViewModel(
                    id:               "activity.005",
                    timestamp:        "Day 11, 11:00",
                    description:      "[Placeholder] Contract with Acme Corp signed",
                    categoryLabel:    "Contracts",
                    semanticState:    "info",
                    drillDownRouteId: ScreenIds.Contracts,
                    isClickable:      true),
            };
        }

        private static UpNextItemViewModel BuildPlaceholderUpNext()
        {
            return new UpNextItemViewModel(
                title:       "Upcoming Milestones",
                hasItems:    true,
                emptyMessage: "No upcoming milestones.",
                items:       new[]
                {
                    "[Placeholder] Day 20 — MVP feature complete",
                    "[Placeholder] Day 30 — Beta launch readiness review",
                    "[Placeholder] Day 45 — Public launch target",
                });
        }
    }
}
