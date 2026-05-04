namespace Project.Presentation.UI.Screens.Market
{
    /// <summary>
    /// Display data for one market category card or row.
    /// Immutable after construction. No Unity dependencies.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// SemanticState drives USS state classes applied by the View (e.g. "high-growth", "high-competition").
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class MarketCategoryRowViewModel
    {
        /// <summary>Stable ID for this market category (e.g. "category.consumer_os").</summary>
        public string Id { get; }

        /// <summary>Display name of the market category.</summary>
        public string CategoryName { get; }

        /// <summary>Formatted demand label (e.g. "High", "Medium", "42 M units / year").</summary>
        public string Demand { get; }

        /// <summary>Formatted growth rate label (e.g. "+12% YoY").</summary>
        public string GrowthRate { get; }

        /// <summary>Formatted competitive intensity label (e.g. "Intense", "Moderate").</summary>
        public string CompetitiveIntensity { get; }

        /// <summary>Formatted current leaders label (e.g. "Apex Corp, Nova Systems").</summary>
        public string CurrentLeaders { get; }

        /// <summary>Formatted technology expectations label (e.g. "High — cutting-edge required").</summary>
        public string TechnologyExpectations { get; }

        /// <summary>Formatted price sensitivity label (e.g. "Low", "High").</summary>
        public string PriceSensitivity { get; }

        /// <summary>Formatted support expectations label (e.g. "24/7 enterprise SLAs expected").</summary>
        public string SupportExpectations { get; }

        /// <summary>Formatted trend modifiers label (e.g. "AI adoption boost, Supply chain risk").</summary>
        public string TrendModifiers { get; }

        /// <summary>
        /// Semantic visual state string used to apply USS state classes on the View.
        /// Examples: "default", "high-growth", "high-competition", "player-present", "hidden".
        /// Must not be a raw colour or pixel value.
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when this row can be clicked to open the category detail drawer.</summary>
        public bool IsClickable { get; }

        public MarketCategoryRowViewModel(
            string id,
            string categoryName,
            string demand,
            string growthRate,
            string competitiveIntensity,
            string currentLeaders,
            string technologyExpectations,
            string priceSensitivity,
            string supportExpectations,
            string trendModifiers,
            string semanticState,
            bool isClickable)
        {
            Id                      = id;
            CategoryName            = categoryName;
            Demand                  = demand;
            GrowthRate              = growthRate;
            CompetitiveIntensity    = competitiveIntensity;
            CurrentLeaders          = currentLeaders;
            TechnologyExpectations  = technologyExpectations;
            PriceSensitivity        = priceSensitivity;
            SupportExpectations     = supportExpectations;
            TrendModifiers          = trendModifiers;
            SemanticState           = semanticState;
            IsClickable             = isClickable;
        }
    }
}
