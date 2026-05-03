using System.Collections.Generic;
using Project.Core.Definitions.Market;

namespace Project.Core.Runtime.Market
{
    /// <summary>
    /// Per-save profile data for an AI competitor.
    /// Contains stable identity and strategic configuration that does not change
    /// during a play session unless the simulation explicitly modifies it.
    /// Mutable fields are intentional — strategy shifts may occur over long sessions.
    /// </summary>
    public sealed class CompetitorProfile
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable ID for this competitor. Persisted across saves.</summary>
        public string Id;

        /// <summary>Display name of the competitor. May change without breaking save data.</summary>
        public string Name;

        // -------------------------------------------------------------------------
        // Strategy
        // -------------------------------------------------------------------------

        /// <summary>Archetype classification that drives overall competitor behaviour.</summary>
        public CompetitorArchetype Archetype;

        /// <summary>Market categories this competitor actively competes in.</summary>
        public List<MarketCategoryType> MarketFocus;

        /// <summary>
        /// Willingness to take risky moves (aggressive expansion, large R&D bets).
        /// Range: 0–100. Higher = more risk-tolerant.
        /// </summary>
        public int RiskAppetite;

        /// <summary>The pricing strategy this competitor tends to follow.</summary>
        public CompetitorPricingStyle PricingStyle;

        /// <summary>The marketing approach this competitor tends to follow.</summary>
        public CompetitorMarketingStyle MarketingStyle;
    }
}
