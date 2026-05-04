namespace Project.Presentation.UI.Screens.Competitors
{
    /// <summary>
    /// Display data for one competitor table row on the Competitors screen (screen.competitors).
    /// Immutable after construction. No Unity dependencies.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompetitorRowViewModel
    {
        /// <summary>Stable ID used to identify this competitor across screens and drawers.</summary>
        public string Id { get; }

        /// <summary>Display name of the competitor company, e.g. "Apex Systems".</summary>
        public string CompanyName { get; }

        /// <summary>
        /// Competitor archetype display label, e.g. "Incumbent Giant", "Aggressive Startup".
        /// Possible values per GDD: Incumbent Giant, Aggressive Startup, Research Lab,
        /// Hardware Manufacturer, Enterprise Specialist, Consumer Brand, Low-Cost Competitor, Platform Holder.
        /// </summary>
        public string Archetype { get; }

        /// <summary>Primary market segment the competitor focuses on, e.g. "Enterprise Software".</summary>
        public string MainMarket { get; }

        /// <summary>Formatted reputation score or label, e.g. "87 / 100" or "High".</summary>
        public string Reputation { get; }

        /// <summary>Formatted product count display value, e.g. "12" or "12 products".</summary>
        public string ProductCount { get; }

        /// <summary>Display label for the competitor's most recent product launch, e.g. "Q3 2025 — DataCore Pro".</summary>
        public string RecentLaunch { get; }

        /// <summary>Formatted market position label, e.g. "Market Leader", "Challenger", "Niche Player".</summary>
        public string MarketPosition { get; }

        /// <summary>
        /// Trend direction display label, e.g. "Growing", "Stable", "Declining".
        /// Drives the trend arrow or chip in the row.
        /// </summary>
        public string Trend { get; }

        /// <summary>
        /// Semantic visual state for this row, e.g. "neutral", "warning", "dominant", "unknown".
        /// Used by the View to apply the correct USS state class without hardcoding colours in C#.
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when this row can be clicked to open the competitor detail drawer.</summary>
        public bool IsClickable { get; }

        public CompetitorRowViewModel(
            string id,
            string companyName,
            string archetype,
            string mainMarket,
            string reputation,
            string productCount,
            string recentLaunch,
            string marketPosition,
            string trend,
            string semanticState,
            bool isClickable)
        {
            Id             = id;
            CompanyName    = companyName;
            Archetype      = archetype;
            MainMarket     = mainMarket;
            Reputation     = reputation;
            ProductCount   = productCount;
            RecentLaunch   = recentLaunch;
            MarketPosition = marketPosition;
            Trend          = trend;
            SemanticState  = semanticState;
            IsClickable    = isClickable;
        }
    }
}
