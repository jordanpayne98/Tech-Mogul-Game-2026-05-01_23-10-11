using Project.Core.Definitions.Market;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Market
{
    /// <summary>
    /// Mutable runtime state for a product released by an AI competitor.
    /// Lightweight proxy: tracks market positioning only. No revenue, users,
    /// review lifecycle, finance, support metrics, or product phase lifecycle.
    /// MarketShareBasisPoints uses 0–10000 range (10000 = 100% share).
    /// QualityScore and MarketingStrength use 0–100 range.
    /// Defined in Plan 2J, GDD_10.
    /// </summary>
    public sealed class CompetitorProductRuntimeState
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable GUID ID for this competitor product instance.</summary>
        public string Id;

        /// <summary>Stable ID of the competitor this product belongs to. References CompetitorProfile.Id.</summary>
        public string CompetitorId;

        /// <summary>Display name of the competitor product.</summary>
        public string Name;

        // -------------------------------------------------------------------------
        // Market Category
        // -------------------------------------------------------------------------

        /// <summary>The market category this product targets.</summary>
        public MarketCategoryType Category;

        // -------------------------------------------------------------------------
        // Competitive Positioning
        // -------------------------------------------------------------------------

        /// <summary>Product price in minor currency units (e.g. pence). Must be > 0.</summary>
        public long PriceMinorUnits;

        /// <summary>
        /// Perceived product quality. Range: 0–100.
        /// Used as input to the market share formula.
        /// </summary>
        public int QualityScore;

        /// <summary>
        /// Relative marketing presence. Range: 0–100.
        /// Derived from marketing style and cash strength at launch.
        /// </summary>
        public int MarketingStrength;

        /// <summary>
        /// Market share expressed in basis points (0–10000).
        /// Written monthly by CompetitorTickProcessor market share resolution.
        /// </summary>
        public int MarketShareBasisPoints;

        // -------------------------------------------------------------------------
        // Lifecycle
        // -------------------------------------------------------------------------

        /// <summary>The in-game date this product was launched.</summary>
        public GameDateTime LaunchDate;

        /// <summary>
        /// Whether this product is actively competing in the market.
        /// For MVP, competitor products are never retired (always true after launch).
        /// </summary>
        public bool IsActive;
    }
}
