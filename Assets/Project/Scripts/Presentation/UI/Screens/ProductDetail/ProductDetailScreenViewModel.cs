using System.Collections.Generic;
using Project.Application;

namespace Project.Presentation.UI.Screens.ProductDetail
{
    /// <summary>
    /// Stable tab ID constants for the Product Detail screen tab bar.
    /// Used in VisibleTabs and ActiveTabId to avoid inline string literals.
    /// </summary>
    public static class ProductDetailTabIds
    {
        public const string Overview    = "tab.overview";
        public const string Development = "tab.development";
        public const string Quality     = "tab.quality";
        public const string Budget      = "tab.budget";
        public const string Marketing   = "tab.marketing";
        public const string Support     = "tab.support";
        public const string Reports     = "tab.reports";
        public const string History     = "tab.history";
        public const string Competitors = "tab.competitors";
    }

    /// <summary>
    /// Top-level aggregate ViewModel for the Product Detail screen (screen.product_detail).
    /// Immutable after construction. No Unity dependencies.
    /// All state-changing action properties (CanLaunch, CanDelay, CanCancel) are false in Phase 5;
    /// lifecycle state changes belong to Core/Application and will be wired in a later phase.
    /// [Placeholder] — static data only in Phase 5.
    /// </summary>
    public sealed class ProductDetailScreenViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Screen title, e.g. the product name.</summary>
        public string ScreenTitle { get; }

        /// <summary>Screen subtitle, e.g. status line or breadcrumb hint.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while data is being loaded; View shows loading state.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data error occurred; View shows error state.</summary>
        public bool HasError { get; }

        /// <summary>Error message to display when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty state when no product data exists.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty state.</summary>
        public string EmptyStateBody { get; }

        // ── Header ───────────────────────────────────────────────────────────

        /// <summary>Stable product definition ID, e.g. "product.example".</summary>
        public string ProductId { get; }

        /// <summary>Display name of the product, e.g. "Nexus OS".</summary>
        public string ProductName { get; }

        /// <summary>Pre-formatted product family, e.g. "Consumer Software".</summary>
        public string Family { get; }

        /// <summary>Pre-formatted product type, e.g. "Operating System".</summary>
        public string Type { get; }

        /// <summary>Current lifecycle status label, e.g. "In Development".</summary>
        public string Status { get; }

        /// <summary>Target market label, e.g. "Enterprise".</summary>
        public string TargetMarket { get; }

        /// <summary>Customer segment label, e.g. "SMB".</summary>
        public string CustomerSegment { get; }

        /// <summary>Price model label, e.g. "Subscription".</summary>
        public string PriceModel { get; }

        /// <summary>Pre-formatted launch target, e.g. "Q2, Year 2".</summary>
        public string LaunchTarget { get; }

        // ── Tabs ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Ordered list of tab IDs that should be rendered.
        /// Use <see cref="ProductDetailTabIds"/> constants.
        /// </summary>
        public IReadOnlyList<string> VisibleTabs { get; }

        /// <summary>
        /// Stable ID of the currently active tab.
        /// Use <see cref="ProductDetailTabIds"/> constants.
        /// </summary>
        public string ActiveTabId { get; }

        // ── Tab content ──────────────────────────────────────────────────────

        /// <summary>Overview metric cards shown in the Overview tab.</summary>
        public IReadOnlyList<ProductMetricCardViewModel> OverviewCards { get; }

        /// <summary>Risk and quality summary shown in the right summary panel.</summary>
        public ProductRiskSummaryViewModel RiskSummary { get; }

        /// <summary>History rows shown in the History tab.</summary>
        public IReadOnlyList<ProductHistoryRowViewModel> HistoryRows { get; }

        // ── Actions ──────────────────────────────────────────────────────────

        /// <summary>
        /// True when the product is eligible for launch.
        /// Phase 5: always false — lifecycle changes belong to Application/Core.
        /// </summary>
        public bool CanLaunch { get; }

        /// <summary>
        /// True when the product launch can be delayed.
        /// Phase 5: always false — lifecycle changes belong to Application/Core.
        /// </summary>
        public bool CanDelay { get; }

        /// <summary>
        /// True when the product can be cancelled.
        /// Phase 5: always false — lifecycle changes belong to Application/Core.
        /// </summary>
        public bool CanCancel { get; }

        /// <summary>
        /// Screen route ID to navigate back to, e.g. <see cref="ScreenIds.Products"/>.
        /// </summary>
        public string BackRouteId { get; }

        // ── Warning states ───────────────────────────────────────────────────

        /// <summary>True when no team is assigned to this product.</summary>
        public bool HasNoAssignedTeam { get; }

        /// <summary>True when the product's overall risk score is above the warning threshold.</summary>
        public bool HasHighRisk { get; }

        /// <summary>
        /// Semantic state string for the screen as a whole: "normal", "warning", "danger", or "success".
        /// Used to drive top-level USS state classes.
        /// </summary>
        public string SemanticState { get; }

        public ProductDetailScreenViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            string productId,
            string productName,
            string family,
            string type,
            string status,
            string targetMarket,
            string customerSegment,
            string priceModel,
            string launchTarget,
            IReadOnlyList<string> visibleTabs,
            string activeTabId,
            IReadOnlyList<ProductMetricCardViewModel> overviewCards,
            ProductRiskSummaryViewModel riskSummary,
            IReadOnlyList<ProductHistoryRowViewModel> historyRows,
            bool canLaunch,
            bool canDelay,
            bool canCancel,
            string backRouteId,
            bool hasNoAssignedTeam,
            bool hasHighRisk,
            string semanticState)
        {
            ScreenTitle       = screenTitle;
            ScreenSubtitle    = screenSubtitle;
            IsLoading         = isLoading;
            HasError          = hasError;
            ErrorMessage      = errorMessage;
            EmptyStateTitle   = emptyStateTitle;
            EmptyStateBody    = emptyStateBody;
            ProductId         = productId;
            ProductName       = productName;
            Family            = family;
            Type              = type;
            Status            = status;
            TargetMarket      = targetMarket;
            CustomerSegment   = customerSegment;
            PriceModel        = priceModel;
            LaunchTarget      = launchTarget;
            VisibleTabs       = visibleTabs;
            ActiveTabId       = activeTabId;
            OverviewCards     = overviewCards;
            RiskSummary       = riskSummary;
            HistoryRows       = historyRows;
            CanLaunch         = canLaunch;
            CanDelay          = canDelay;
            CanCancel         = canCancel;
            BackRouteId       = backRouteId;
            HasNoAssignedTeam = hasNoAssignedTeam;
            HasHighRisk       = hasHighRisk;
            SemanticState     = semanticState;
        }
    }
}
