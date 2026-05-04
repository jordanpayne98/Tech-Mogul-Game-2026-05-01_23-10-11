using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.FounderPortal
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Founder Portal dashboard screen (screen.founder_portal).
    /// Immutable after construction. No Unity dependencies.
    /// Created by FounderPortalController and passed to FounderPortalView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class FounderPortalViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Founder Portal".</summary>
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

        // ── Dashboard content ────────────────────────────────────────────────

        /// <summary>KPI cards displayed in the top KPI grid.</summary>
        public IReadOnlyList<KpiCardViewModel> KpiCards { get; }

        /// <summary>Main summary cards (Inbox, Products, Team, Hiring, Milestones, Market, Infrastructure).</summary>
        public IReadOnlyList<DashboardCardViewModel> DashboardCards { get; }

        /// <summary>Recent activity rows displayed in the activity strip.</summary>
        public IReadOnlyList<RecentActivityItemViewModel> RecentActivityItems { get; }

        /// <summary>Quick action entry points displayed in the quick action strip.</summary>
        public IReadOnlyList<QuickActionViewModel> QuickActions { get; }

        /// <summary>Upcoming milestones card data.</summary>
        public UpNextItemViewModel UpNextItem { get; }

        // ── Warning states ───────────────────────────────────────────────────

        /// <summary>True when cash runway is below the warning threshold.</summary>
        public bool HasLowRunwayWarning { get; }

        /// <summary>True when team or active-work workload exceeds the warning threshold.</summary>
        public bool HasHighWorkloadWarning { get; }

        /// <summary>True when there are unread items in the decision inbox.</summary>
        public bool HasUnreadDecisions { get; }

        /// <summary>True when one or more infrastructure components are at risk.</summary>
        public bool HasInfrastructureRisk { get; }

        public FounderPortalViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            IReadOnlyList<KpiCardViewModel> kpiCards,
            IReadOnlyList<DashboardCardViewModel> dashboardCards,
            IReadOnlyList<RecentActivityItemViewModel> recentActivityItems,
            IReadOnlyList<QuickActionViewModel> quickActions,
            UpNextItemViewModel upNextItem,
            bool hasLowRunwayWarning,
            bool hasHighWorkloadWarning,
            bool hasUnreadDecisions,
            bool hasInfrastructureRisk)
        {
            ScreenTitle            = screenTitle;
            ScreenSubtitle         = screenSubtitle;
            IsLoading              = isLoading;
            HasError               = hasError;
            ErrorMessage           = errorMessage;
            EmptyStateTitle        = emptyStateTitle;
            EmptyStateBody         = emptyStateBody;
            KpiCards               = kpiCards;
            DashboardCards         = dashboardCards;
            RecentActivityItems    = recentActivityItems;
            QuickActions           = quickActions;
            UpNextItem             = upNextItem;
            HasLowRunwayWarning    = hasLowRunwayWarning;
            HasHighWorkloadWarning = hasHighWorkloadWarning;
            HasUnreadDecisions     = hasUnreadDecisions;
            HasInfrastructureRisk  = hasInfrastructureRisk;
        }
    }
}
