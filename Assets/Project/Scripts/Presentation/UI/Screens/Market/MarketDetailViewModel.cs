using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Market
{
    /// <summary>
    /// Display data for the category detail drawer or modal.
    /// Shown when the player selects a market category row.
    /// Immutable after construction. No Unity dependencies.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class MarketDetailViewModel
    {
        /// <summary>Stable ID of the category being detailed (e.g. "category.consumer_os").</summary>
        public string CategoryId { get; }

        /// <summary>Display name of the market category.</summary>
        public string CategoryName { get; }

        /// <summary>Formatted demand label (e.g. "High", "42 M units / year").</summary>
        public string Demand { get; }

        /// <summary>Formatted growth rate label (e.g. "+12% YoY").</summary>
        public string GrowthRate { get; }

        /// <summary>Formatted competitive intensity label (e.g. "Intense", "Moderate").</summary>
        public string CompetitiveIntensity { get; }

        // ── Detail content ───────────────────────────────────────────────────

        /// <summary>Customer preference entries for the segment breakdown panel.</summary>
        public IReadOnlyList<CustomerPreferenceViewModel> CustomerPreferences { get; }

        /// <summary>Product ranking rows for this category's leaderboard.</summary>
        public IReadOnlyList<MarketRankingRowViewModel> ProductRankings { get; }

        /// <summary>Formatted average market price label (e.g. "$349 avg. retail").</summary>
        public string AveragePrice { get; }

        /// <summary>Display labels for recently launched products in this category.</summary>
        public IReadOnlyList<string> RecentLaunches { get; }

        /// <summary>Formatted label describing the player company's current position in this category.</summary>
        public string PlayerPosition { get; }

        /// <summary>Display labels for opportunity signals identified in this category.</summary>
        public IReadOnlyList<string> OpportunityIndicators { get; }

        /// <summary>Display labels for risk signals identified in this category.</summary>
        public IReadOnlyList<string> RiskIndicators { get; }

        public MarketDetailViewModel(
            string categoryId,
            string categoryName,
            string demand,
            string growthRate,
            string competitiveIntensity,
            IReadOnlyList<CustomerPreferenceViewModel> customerPreferences,
            IReadOnlyList<MarketRankingRowViewModel> productRankings,
            string averagePrice,
            IReadOnlyList<string> recentLaunches,
            string playerPosition,
            IReadOnlyList<string> opportunityIndicators,
            IReadOnlyList<string> riskIndicators)
        {
            CategoryId            = categoryId;
            CategoryName          = categoryName;
            Demand                = demand;
            GrowthRate            = growthRate;
            CompetitiveIntensity  = competitiveIntensity;
            CustomerPreferences   = customerPreferences;
            ProductRankings       = productRankings;
            AveragePrice          = averagePrice;
            RecentLaunches        = recentLaunches;
            PlayerPosition        = playerPosition;
            OpportunityIndicators = opportunityIndicators;
            RiskIndicators        = riskIndicators;
        }
    }
}
