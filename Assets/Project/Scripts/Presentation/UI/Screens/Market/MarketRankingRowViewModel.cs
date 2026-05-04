namespace Project.Presentation.UI.Screens.Market
{
    /// <summary>
    /// Display data for one product ranking row in the market leaderboard.
    /// Used in both the Market overview and the category detail drawer.
    /// Immutable after construction. No Unity dependencies.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// SemanticState drives USS state classes applied by the View (e.g. "player-product", "competitor").
    /// DrillDownRouteId is empty when no linked product or competitor detail screen exists.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class MarketRankingRowViewModel
    {
        /// <summary>Formatted rank label (e.g. "#1", "#2").</summary>
        public string Rank { get; }

        /// <summary>Display name of the ranked product.</summary>
        public string ProductName { get; }

        /// <summary>Display name of the company that owns the product.</summary>
        public string CompanyName { get; }

        /// <summary>Formatted score or market share label (e.g. "34.2%", "92 pts").</summary>
        public string Score { get; }

        /// <summary>
        /// Semantic visual state string used to apply USS state classes on the View.
        /// Examples: "player-product", "competitor", "default".
        /// Must not be a raw colour or pixel value.
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when this row represents a product owned by the player company.</summary>
        public bool IsPlayerProduct { get; }

        /// <summary>
        /// Stable route ID to open a product or competitor detail screen when clicked.
        /// Empty string when no drill-down target exists.
        /// </summary>
        public string DrillDownRouteId { get; }

        /// <summary>True when this row can be clicked to navigate to a detail view.</summary>
        public bool IsClickable { get; }

        public MarketRankingRowViewModel(
            string rank,
            string productName,
            string companyName,
            string score,
            string semanticState,
            bool isPlayerProduct,
            string drillDownRouteId,
            bool isClickable)
        {
            Rank             = rank;
            ProductName      = productName;
            CompanyName      = companyName;
            Score            = score;
            SemanticState    = semanticState;
            IsPlayerProduct  = isPlayerProduct;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
