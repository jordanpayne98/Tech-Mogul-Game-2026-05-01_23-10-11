using System.Collections.Generic;
using Project.Core.Definitions.Market;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Market
{
    /// <summary>
    /// Mutable runtime state for a market trend.
    /// Tracks the strength, affected categories, lifecycle dates, and active status
    /// of a macro-level economic or technology trend.
    /// EstimatedEndDate is nullable — some trends have no predicted end date.
    /// </summary>
    public sealed class TrendRuntimeState
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable ID for this trend instance.</summary>
        public string Id;

        /// <summary>The type of macro trend this represents.</summary>
        public TrendType Type;

        // -------------------------------------------------------------------------
        // State
        // -------------------------------------------------------------------------

        /// <summary>Original strength at trend creation. Used for decay formula. Never modified after creation.</summary>
        public int InitialStrength;

        /// <summary>Current strength of the trend. Range: 0–100. Higher = stronger market effect.</summary>
        public int Strength;

        /// <summary>Market categories affected by this trend.</summary>
        public List<MarketCategoryType> AffectedCategories;

        // -------------------------------------------------------------------------
        // Lifecycle
        // -------------------------------------------------------------------------

        /// <summary>In-game date when this trend became active.</summary>
        public GameDateTime StartDate;

        /// <summary>
        /// Estimated in-game date when this trend is expected to end.
        /// Null if the trend has no defined end date.
        /// </summary>
        public GameDateTime EstimatedEndDate;

        /// <summary>
        /// Whether this trend is currently active.
        /// Inactive trends are retained for historical reference but do not affect simulation.
        /// </summary>
        public bool IsActive;
    }
}
