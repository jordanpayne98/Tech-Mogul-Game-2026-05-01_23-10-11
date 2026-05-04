namespace Project.Presentation.UI.Screens.Finance
{
    /// <summary>
    /// Pure display-data class for a single row in the product revenue history table.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// DrillDownRouteId is empty string when the row is not clickable.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductRevenueRowViewModel
    {
        /// <summary>Stable product ID used for navigation, e.g. "product.example_001".</summary>
        public string ProductId { get; }

        /// <summary>Display name of the product, e.g. "CloudSync Pro".</summary>
        public string ProductName { get; }

        /// <summary>Pre-formatted revenue earned this month, e.g. "$18,000".</summary>
        public string RevenueThisMonth { get; }

        /// <summary>Pre-formatted all-time total revenue for this product, e.g. "$142,000".</summary>
        public string RevenueTotal { get; }

        /// <summary>Pre-formatted trend text, e.g. "+5% vs last month". Empty string if no trend.</summary>
        public string TrendText { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        /// <summary>Route ID for drill-down navigation to product detail. Empty string when not navigable.</summary>
        public string DrillDownRouteId { get; }

        /// <summary>Whether the row responds to click/tap input.</summary>
        public bool IsClickable { get; }

        public ProductRevenueRowViewModel(
            string productId,
            string productName,
            string revenueThisMonth,
            string revenueTotal,
            string trendText,
            string semanticState,
            string drillDownRouteId,
            bool isClickable)
        {
            ProductId        = productId;
            ProductName      = productName;
            RevenueThisMonth = revenueThisMonth;
            RevenueTotal     = revenueTotal;
            TrendText        = trendText;
            SemanticState    = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
