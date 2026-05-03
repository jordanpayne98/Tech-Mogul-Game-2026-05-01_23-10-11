using System.Collections.Generic;
using Project.Core.Definitions.Product;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Product
{
    /// <summary>
    /// Mutable runtime state for a product. Tracks live simulation data such as
    /// lifecycle status, revenue, user counts, scores, and market position.
    /// MarketShareBasisPoints uses a 0–10000 range (100 = 1%).
    /// ProgressPercent and all score values use a 0–100 range.
    /// All revenue and monetary fields use minor currency units (e.g. cents).
    /// </summary>
    public sealed class ProductRuntimeState
    {
        /// <summary>Stable ID of the ProductProfile this state belongs to.</summary>
        public string ProductId;

        /// <summary>Current lifecycle stage of the product.</summary>
        public ProductStatus Status;

        /// <summary>Development or milestone progress as a percentage (0–100).</summary>
        public int ProgressPercent;

        /// <summary>Stable IDs of teams currently assigned to work on this product.</summary>
        public List<string> AssignedTeamIds;

        /// <summary>The in-game date the product was launched. Null if not yet launched.</summary>
        public GameDateTime LaunchDate;

        /// <summary>Cumulative revenue earned since launch in minor currency units.</summary>
        public long TotalRevenueMinorUnits;

        /// <summary>Revenue earned in the current simulation month in minor currency units.</summary>
        public long CurrentMonthRevenueMinorUnits;

        /// <summary>Total units sold across all time.</summary>
        public long UnitsSoldTotal;

        /// <summary>Units sold in the current simulation month.</summary>
        public long UnitsSoldThisMonth;

        /// <summary>Number of users currently active with this product.</summary>
        public int ActiveUsers;

        /// <summary>Aggregate review score (0–100).</summary>
        public int ReviewScore;

        /// <summary>Review score based on recent reviews only (0–100).</summary>
        public int RecentReviewScore;

        /// <summary>
        /// Market share expressed in basis points (0–10000).
        /// 100 basis points = 1% market share.
        /// </summary>
        public int MarketShareBasisPoints;

        /// <summary>Per-dimension score values. Each dimension uses a 0–100 range.</summary>
        public Dictionary<ProductScoreDimension, int> ScoreValues;

        /// <summary>
        /// Stable IDs referencing monthly revenue history records.
        /// Ordered by simulation month for audit and charting purposes.
        /// </summary>
        public List<string> MonthlyRevenueHistoryIds;
    }
}
