using System.Collections.Generic;
using Project.Core.Definitions.Market;

namespace Project.Core.Runtime.Market
{
    /// <summary>
    /// Mutable runtime state for a single market category.
    /// Tracks current demand, customer preference weights, competitive intensity,
    /// and active trend references. Updated by the market simulation each tick.
    /// </summary>
    public sealed class MarketCategoryRuntimeState
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable ID for this market category state entry.</summary>
        public string Id;

        /// <summary>The market category this state represents.</summary>
        public MarketCategoryType CategoryType;

        // -------------------------------------------------------------------------
        // Demand
        // -------------------------------------------------------------------------

        /// <summary>Total addressable demand in this category. Arbitrary unit scale defined by simulation.</summary>
        public int TotalDemand;

        /// <summary>
        /// Growth rate expressed in basis points (0–10000, where 10000 = 100%).
        /// Positive = growing market, negative = shrinking market.
        /// </summary>
        public int GrowthRateBasisPoints;

        /// <summary>
        /// Weighted demand distribution across customer segments.
        /// Values represent relative weight — not percentages. Simulation normalises on read.
        /// </summary>
        public Dictionary<CustomerSegment, int> SegmentDemandWeights;

        // -------------------------------------------------------------------------
        // Customer Preferences
        // -------------------------------------------------------------------------

        /// <summary>
        /// Current customer preference weights for this market category.
        /// Higher values indicate stronger preference for that dimension.
        /// </summary>
        public Dictionary<CustomerPreference, int> CustomerPreferences;

        // -------------------------------------------------------------------------
        // Competitive State
        // -------------------------------------------------------------------------

        /// <summary>Overall competitive intensity in this category. Range: 0–100.</summary>
        public int CompetitiveIntensity;

        /// <summary>
        /// Stable IDs of products currently leading this market category.
        /// Used for market share resolution and competitive pressure calculations.
        /// </summary>
        public List<string> LeaderProductIds;

        // -------------------------------------------------------------------------
        // Expectation Thresholds
        // -------------------------------------------------------------------------

        /// <summary>How advanced the technology expectation is for products in this category. Range: 0–100.</summary>
        public int TechnologyExpectation;

        /// <summary>How sensitive customers in this category are to price. Range: 0–100.</summary>
        public int PriceSensitivity;

        /// <summary>How responsive customers in this category are to marketing. Range: 0–100.</summary>
        public int MarketingSensitivity;

        /// <summary>Expected level of post-sale support quality. Range: 0–100.</summary>
        public int SupportExpectation;

        // -------------------------------------------------------------------------
        // Active Trends
        // -------------------------------------------------------------------------

        /// <summary>Stable IDs of trends currently affecting this market category.</summary>
        public List<string> ActiveTrendIds;
    }
}
