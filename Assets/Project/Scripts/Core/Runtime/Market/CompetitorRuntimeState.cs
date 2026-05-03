using System.Collections.Generic;
using Project.Core.Definitions.Market;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Market
{
    /// <summary>
    /// Mutable runtime state for an AI competitor.
    /// Tracks current strength indicators and market share per category.
    /// CashStrength is a relative indicator (0–100), not a currency amount.
    /// MarketShareBasisPoints uses 0–10000 range (10000 = 100% share).
    /// </summary>
    public sealed class CompetitorRuntimeState
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable ID of the competitor this state belongs to. References CompetitorProfile.Id.</summary>
        public string CompetitorId;

        // -------------------------------------------------------------------------
        // Strength Indicators (0–100 relative scale unless otherwise noted)
        // -------------------------------------------------------------------------

        /// <summary>
        /// Relative cash position indicator. Range: 0–100.
        /// Not actual currency — represents relative financial health of the competitor.
        /// </summary>
        public int CashStrength;

        /// <summary>Brand and public reputation strength. Range: 0–100.</summary>
        public int Reputation;

        /// <summary>Stable IDs of products this competitor has released.</summary>
        public List<string> ProductIds;

        /// <summary>Ability to attract and retain talent. Range: 0–100.</summary>
        public int HiringStrength;

        /// <summary>Effectiveness of R&D investment. Range: 0–100.</summary>
        public int ResearchStrength;

        /// <summary>Speed at which the competitor releases new products. Range: 0–100.</summary>
        public int LaunchCadence;

        /// <summary>Date of the competitor's most recent product launch. Null until first launch. Used for cadence gating.</summary>
        public GameDateTime LastLaunchDate;

        // -------------------------------------------------------------------------
        // Market Share
        // -------------------------------------------------------------------------

        /// <summary>
        /// Market share per category expressed in basis points (0–10000, where 10000 = 100%).
        /// Only categories the competitor is active in will have entries.
        /// </summary>
        public Dictionary<MarketCategoryType, int> MarketShareBasisPoints;
    }
}
