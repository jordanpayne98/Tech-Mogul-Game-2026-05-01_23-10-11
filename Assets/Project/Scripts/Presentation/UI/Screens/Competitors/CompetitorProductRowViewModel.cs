namespace Project.Presentation.UI.Screens.Competitors
{
    /// <summary>
    /// Display data for one competitor product row inside the competitor detail drawer.
    /// Immutable after construction. No Unity dependencies.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompetitorProductRowViewModel
    {
        /// <summary>Display name of the competitor product, e.g. "DataCore Pro".</summary>
        public string ProductName { get; }

        /// <summary>Product type display label, e.g. "Software", "Hardware", "Platform".</summary>
        public string Type { get; }

        /// <summary>Target market segment of this product, e.g. "Enterprise", "Consumer".</summary>
        public string Market { get; }

        /// <summary>Formatted competitive score or rating display, e.g. "82 / 100" or "High".</summary>
        public string Score { get; }

        /// <summary>Current product status label, e.g. "Active", "Discontinued", "Upcoming".</summary>
        public string Status { get; }

        /// <summary>
        /// Semantic visual state for this product row, e.g. "neutral", "warning", "dominant".
        /// Used by the View to apply the correct USS state class without hardcoding colours in C#.
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when this row can be clicked to navigate to a product comparison or detail view.</summary>
        public bool IsClickable { get; }

        /// <summary>
        /// Stable route ID used when IsClickable is true, e.g. "screen.product_detail" or "screen.market_detail".
        /// Empty string when IsClickable is false.
        /// </summary>
        public string DrillDownRouteId { get; }

        public CompetitorProductRowViewModel(
            string productName,
            string type,
            string market,
            string score,
            string status,
            string semanticState,
            bool isClickable,
            string drillDownRouteId)
        {
            ProductName      = productName;
            Type             = type;
            Market           = market;
            Score            = score;
            Status           = status;
            SemanticState    = semanticState;
            IsClickable      = isClickable;
            DrillDownRouteId = drillDownRouteId;
        }
    }
}
